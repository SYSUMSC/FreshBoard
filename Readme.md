# MSC 2018 Freshman Portal System

## 项目结构
#### 前端：React
```
./ClientApp
```
#### 后端：.NET Core 2.1
```
SDK 下载：https://www.microsoft.com/net/download
```
#### 其他说明
```
mscfreshman
│   .gitignore -- git 忽略文件
│   app.db -- 网站数据库
│   appsettings.Development.json -- 应用配置（调试用）
│   appsettings.json -- 应用配置
│   mscfreshman.csproj -- 项目属性及配置
│   mscfreshman.sln -- 解决方案
│   Program.cs -- 后端主程序
│   Readme.md -- README
│   Startup.cs -- web 启动配置
│       
├───bin -- 编译输出目录
│   
├───ClientApp -- 前端（React）
│   ├───public -- 静态页面
│   └───src -- 前端代码
│   
├───Controllers -- 后端控制器
│   
├───Data -- 数据库相关
│   │   ApplicationDbContext.cs -- 数据库强类型上下文
│   │   
│   ├───Identity -- 账户相关
│   │       FreshBoardUser.cs -- 账户信息字段
│   │       TranslatedIdentityErrorDescriber.cs -- 一些错误的翻译
│   │       
│   └───Migrations -- 数据库合并自动化脚本
│           
├───obj -- .NET Core 包的临时目录，类似 node_modules
│                           
├───Pages -- 内建页面，如内部错误页面（500）
│       
└───Services -- 服务
        EmailSender.cs -- 邮件发信服务器
        SignalRHub.cs -- WebSocket 后端处理函数
```

## Coding & Build
想用什么编辑器就用什么编辑器，以 vsc 举例，直接打开本项目文件夹即可  
需要安装 C# 插件  
如果使用 vs，则直接运行 mscfreshman.sln 即可
#### Restore Packages
```
dotnet restore

cd ./ClientApp
npm install
```
#### Build
```
dotnet build -r 目标系统 -c 发布类型
```
目标系统为系统平台和架构的组合，例如 win-x64、linux-x64 等  
发布类型可为 Debug 或 Release，默认为 Debug

## Run & Debug
vsc 可以直接按 F5 运行调试，注意打开项目文件夹的时候右下角的提示窗，需要选择 Yes 才可以
#### Run
```
dotnet run
```
注意运行此命令前，当前目录需要切换到项目根目录下，且需要将环境变量 ASPNETCORE_ENVIRONMENT 设置为 'Development'  
设置方法：  
```
Windows Powershell: $env:ASPNETCORE_ENVIRONMENT = 'Development'
Windows Command Prompt: set ASPNETCORE_ENVIRONMENT=Development
Linux Bash: export ASPNETCORE_ENVIRONMENT=Development
```
不需要单独运行前端，直接使用此命令即可将网站运行在 http://localhost:5000 和 https://localhost:5001 （如果安装了证书） 上

## Publish
```
dotnet publish -r 目标系统 -c 发布类型
```
发布完成后会生成到 ./bin/发布类型/netcoreapp2.1/目标系统/publish 中  
复制 publish 里面的所有文件到目标机器上，运行 mscfreshman 主程序，网站将会自动启动并运行在 http://localhost:5000 以及 https://localhost:5001 （如果安装了证书），可使用 nginx 等做反向代理将其映射至 80/443 端口
