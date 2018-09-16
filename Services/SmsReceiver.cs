using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dybaseapi.Model.V20170525;
using Aliyun.MNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace mscfreshman
{
    class SmsReceiver
    {
        private static readonly int maxThread = 2;
        private static readonly string accessId = Secrets.AliyunAccessKeyId;
        private static readonly string accessSecret = Secrets.AliyunAccessKeySecret;
        private static readonly string messageType = "SmsUp";
        private static readonly string queueName = "Alicom-Queue-1756501826101097-SmsUp";
        private static readonly string domainForPop = "dybaseapi.aliyuncs.com";
        private static readonly string regionIdForPop = "cn-hangzhou";
        private static readonly string productName = "Dybaseapi";
        private static IAcsClient acsClient = null;

        public static IAcsClient InitAcsClient(string regionIdForPop, string accessId, string accessSecret, string productName, string domainForPop)
        {
            var profile = DefaultProfile.GetProfile(regionIdForPop, accessId, accessSecret);
            DefaultProfile.AddEndpoint(regionIdForPop, regionIdForPop, productName, domainForPop);
            var acsClient = new DefaultAcsClient(profile);
            return acsClient;
        }

        // 初始化环境
        private static void InitData()
        {
            acsClient = InitAcsClient(regionIdForPop, accessId, accessSecret, productName, domainForPop);
        }

        public static void StartThread()
        {
            InitData();
            for (int i = 0; i < maxThread; i++)
            {
                var testTask = new ReceiveThread("PullMessageTask-thread-" + i, messageType, queueName, acsClient);
                var t = new Thread(new ThreadStart(testTask.Handle));
                t.Start();
            }
        }
    }

    class ReceiveThread
    {
        private static readonly object o = new object();
        private readonly int sleepTime = 50;
        public string Name { get; private set; }
        public string MessageType { get; private set; }
        public string QueueName { get; private set; }
        public int TaskID { get; private set; }
        public IAcsClient AcsClient { get; private set; }
        private readonly StreamWriter logs;

        public ReceiveThread(string name, string messageType, string queueName, IAcsClient acsClient)
        {
            Name = name;
            MessageType = messageType;
            QueueName = queueName;
            AcsClient = acsClient;
            var file = new FileStream("./" + name + ".txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            file.Seek(0, SeekOrigin.End);
            logs = new StreamWriter(file) { AutoFlush = true };
        }

        private readonly long bufferTime = 60 * 2;
        private readonly string mnsAccountEndpoint = "https://1943695596114318.mns.cn-hangzhou.aliyuncs.com/";
        private Dictionary<string, QueryTokenForMnsQueueResponse.QueryTokenForMnsQueue_MessageTokenDTO> tokenMap = new Dictionary<string, QueryTokenForMnsQueueResponse.QueryTokenForMnsQueue_MessageTokenDTO>();
        private Dictionary<string, Queue> queueMap = new Dictionary<string, Queue>();

        public QueryTokenForMnsQueueResponse.QueryTokenForMnsQueue_MessageTokenDTO GetTokenByMessageType(IAcsClient acsClient, string messageType)
        {
            var request = new QueryTokenForMnsQueueRequest
            {
                MessageType = messageType
            };
            var queryTokenForMnsQueueResponse = acsClient.GetAcsResponse(request);
            var token = queryTokenForMnsQueueResponse.MessageTokenDTO;
            return token;
        }

        public void Handle()
        {
            while (!Environment.HasShutdownStarted)
            {
                try
                {
                    QueryTokenForMnsQueueResponse.QueryTokenForMnsQueue_MessageTokenDTO token = null;
                    Queue queue = null;
                    lock (o)
                    {
                        if (tokenMap.ContainsKey(MessageType))
                        {
                            token = tokenMap[MessageType];
                        }
                        if (queueMap.ContainsKey(QueueName))
                        {
                            queue = queueMap[QueueName];
                        }
                        var ts = new TimeSpan(0);
                        if (token != null)
                        {
                            var b = Convert.ToDateTime(token.ExpireTime);
                            var c = Convert.ToDateTime(DateTime.Now);
                            ts = b - c;
                        }
                        if (token == null || ts.TotalSeconds < bufferTime || queue == null)
                        {
                            token = GetTokenByMessageType(AcsClient, MessageType);
                            var client = new MNSClient(token.AccessKeyId, token.AccessKeySecret, mnsAccountEndpoint, token.SecurityToken);
                            queue = client.GetNativeQueue(QueueName);
                            if (tokenMap.ContainsKey(MessageType))
                            {
                                tokenMap.Remove(MessageType);
                            }
                            if (queueMap.ContainsKey(QueueName))
                            {
                                queueMap.Remove(QueueName);
                            }
                            tokenMap.Add(MessageType, token);
                            queueMap.Add(QueueName, queue);
                        }
                    }
                    var batchReceiveMessageResponse = queue.BatchReceiveMessage(16);
                    var messages = batchReceiveMessageResponse.Messages;
                    for (int i = 0; i <= messages.Count - 1; i++)
                    {
                        try
                        {
                            var outputb = Convert.FromBase64String(messages[i].Body);
                            var orgStr = Encoding.UTF8.GetString(outputb);

                            Console.WriteLine(orgStr);

                            logs.WriteLine(orgStr);

                            queue.DeleteMessage(messages[i].ReceiptHandle);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Thread.Sleep(sleepTime);
            }
            logs?.Dispose();
        }
    }
}