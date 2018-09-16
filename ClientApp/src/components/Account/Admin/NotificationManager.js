import React, { Component } from "react";
import { ModalHeader, ModalBody, ModalFooter, Modal, Container, Badge, Button, ListGroup, ListGroupItem, FormGroup, Input, Label } from "reactstrap";
import { NotificationEditor } from "./NotificationEditor";
import { Get, Post } from "../../../utils/HttpRequest";

export class NotificationManager extends Component {
    displayName = NotificationManager.name

    constructor(props) {
        super(props);
        this.state = {
            notifications: [],
            loading: true,
            currentPage: 1,
            readIndex: 0,
            showModal: false,
            showSendingModal: false,
            pushUsers: null,
            currentSent: 0,
            errors: []
        };
        this._scrollHandler = this._scrollHandler.bind(this);
        this.newNotification = this.newNotification.bind(this);
        this.togglemsg = this.togglemsg.bind(this);
        this.sendEmailSMS = this.sendEmailSMS.bind(this);
        this.removeNotification = this.removeNotification.bind(this);
        this.togglesending = this.togglesending.bind(this);
        this.showErrors = this.showErrors.bind(this);
        this.updateList = this.updateList.bind(this);
        this.sendEmailSMS = this.sendEmailSMS.bind(this);

        this.sending = false;
        this.retry = 0;

        this.getNotifications();
    }

    componentDidMount() {
        window.addEventListener('scroll', this._scrollHandler);
    }

    componentWillUnmount() {
        window.removeEventListener('scroll', this._scrollHandler);
    }

    _scrollHandler() {
        if (window.scrollY + window.innerHeight > document.body.offsetHeight) {
            if (this.state.loading) return;
            const t = this.state.currentPage;
            this.setState({
                currentPage: this.state.notifications.length === 0 ? t : t + 1,
                loading: true
            });
            this.getNotifications();
        }
    }

    getNotifications(fromStart = false) {
        Get('/Admin/GetNotificationsAsync', {}, { start: fromStart ? 0 : (this.state.currentPage - 1) * 20, count: 20 })
            .then(response => response.json())
            .then(data => {
                if (data.succeeded)
                    this.setState({ notifications: this.state.notifications.concat(data.notifications), loading: false });
                else alert(data.message);
            })
            .catch(() => alert('加载失败'));
    }

    togglemsg(index = 0) {
        if (this.state.showModal) {
            this.setState({ readIndex: 0, showModal: false });
            return;
        }
        this.setState({ readIndex: index, showModal: true });
    }

    togglesending(index = 0) {
        if (this.sending) return;
        if (this.state.showSendingModal) {
            this.updateList();
            return;
        }
        var list = document.getElementById('errorlist');
        if (list !== null)
            list.innerHTML = "";
        this.setState({ readIndex: index, showSendingModal: true, currentSent: 0, pushUsers: null });
        Post('/Admin/GetPushNotificationUsersAsync', {}, { nid: this.state.notifications[this.state.readIndex].id })
            .then(res => res.json())
            .then(data => {
                this.setState({ pushUsers: data });
                if (!data.succeeded) alert(data.message);
            })
            .catch(() => alert('发生未知错误'));
    }

    sendSingleSMS(list, nid, userId, phone, email, requireResponse) {
        Post('/Admin/PushNotificationAsync', {}, {
            nid: nid,
            userId: userId,
            phone: phone,
            email: email,
            requireResponse: requireResponse
        })
            .then(res => res.json())
            .then(data => {
                this.retry = 0;
                this.setState({ currentSent: this.state.currentSent + 1 });
                if (list !== null) {
                    var ele = document.createElement('li');
                    ele.className = "justify-content-between list-group-item";
                    ele.innerHTML = `<p>${userId}</p><p>邮件 ${(data.emailSucceeded ? '√' : '×')} 短信 ${(data.phoneSucceeded ? '√' : '×')}</p>`;
                    list.appendChild(ele);
                }
                if (this.state.currentSent >= this.state.pushUsers.users.length) {
                    this.sending = false;
                    Post('/Admin/SetNotificationPushStatusAsync', {}, { nid: nid });
                    return;
                }
                this.sendSingleSMS(list, nid, this.state.pushUsers.users[this.state.currentSent].id, phone, email, requireResponse);
            })
            .catch(() => {
                this.retry++;
                if (this.retry > 3) {
                    this.setState({ currentSent: this.state.currentSent + 1 });
                    this.retry = 0;
                    if (list !== null) {
                        var ele = document.createElement('li');
                        ele.className = "justify-content-between list-group-item";
                        ele.innerHTML = `<p>${userId}</p><p>邮件 × 短信 ×</p>`;
                        list.appendChild(ele);
                    }
                }
                if (this.state.currentSent >= this.state.pushUsers.users.length) {
                    this.sending = false;
                    Post('/Admin/SetNotificationPushStatusAsync', {}, { nid: nid });
                    return;
                }
                this.sendSingleSMS(list, nid, this.state.pushUsers.users[this.state.currentSent].id, phone, email, requireResponse);
            });
    }

    sendEmailSMS(nid) {
        if (this.sending) return;
        if (this.state.pushUsers === null || this.state.pushUsers.users === null) return;
        if (this.state.notifications.length <= this.state.readIndex) return;
        this.setState({ currentSent: 0 });
        this.sending = true;
        this.retry = 0;
        var email = document.getElementById('sendemail').checked;
        var phone = document.getElementById('sendsms').checked;
        var requireResponse = document.getElementById('requireresponse').checked;
        var list = document.getElementById('errorlist');
        if (list !== null)
            list.innerHTML = "";
        this.sendSingleSMS(list, nid, this.state.pushUsers.users[this.state.currentSent].id, phone, email, requireResponse);
    }

    removeNotification(nid, index) {
        Post('/Admin/RemoveNotificationAsync', {}, { nid: nid })
            .then(res => res.json())
            .then(data => {
                if (data.succeeded) {
                    var ele = document.getElementById('msg_' + index.toString());
                    if (ele !== null) ele.remove();
                }
                else alert(data.message);
            })
            .catch(() => alert('发生未知错误'));
    }

    updateList() {
        this.setState({
            notifications: [],
            loading: true,
            currentPage: 1,
            readIndex: 0,
            showModal: false,
            showSendingModal: false,
            pushUsers: null,
            currentSent: 0,
            errors: []
        });
        this.getNotifications(true);
    }

    getNotificationLayout(notifications) {
        return (
            <div>
                {
                    notifications.length === 0 ? <p>暂无通知</p> :
                        <ListGroup>
                            {
                                notifications.map((x, i) => (
                                    <ListGroupItem className="justify-content-between" id={"msg_" + i.toString()}>
                                        <a href="javascript:void(0)" onClick={() => this.togglemsg(i, x.id)}>{x.title}</a>&nbsp;
                                <Badge pill>{x.time}</Badge>&nbsp;
                                {x.hasPushed ? <Badge pill color="success" id={'pushed_' + i.toString()}>已发送通知邮件/短信</Badge> : <Badge pill color="danger" id={'pushed_' + i.toString()}>未发送通知邮件/短信</Badge>}
                                        <div className="float-right">
                                            <Button color={x.hasPushed ? "secondary" : "primary"} onClick={() => this.togglesending(i)}>发送通知邮件/短信</Button>&nbsp;
                                            <Button color="danger" onClick={() => this.removeNotification(x.id, i)}>删除</Button>
                                        </div>
                                    </ListGroupItem>))
                            }
                        </ListGroup>
                }
            </div>
        );
    }

    newNotification() {
        this.setState({
            readIndex: this.state.notifications.length,
            showModal: true
        });
    }

    showErrors() {
        return (<div style={{ maxHeight: '200px', overflow: 'auto' }}><ListGroup>
            {this.state.errors.map(x => {
                <ListGroupItem className="justify-content-between"><p className="danger">用户 {x.id}：短信 {x.phoneSucceeded ? "√" : "×"} 邮件 {x.emailSucceeded ? "√" : "×"}</p></ListGroupItem>
            })}
        </ListGroup></div>);
    }

    render() {
        let canRead = this.state.notifications.length > this.state.readIndex;

        return (
            <Container>
                <br />
                <h2>通知管理 <Button color="primary" onClick={this.newNotification}>新建</Button></h2>
                {this.getNotificationLayout(this.state.notifications)}
                {this.state.loading ? <div><br /><p>加载中...</p></div> : null}

                <Modal isOpen={this.state.showModal} toggle={this.togglemsg}>
                    <ModalHeader toggle={this.togglemsg}>编辑通知</ModalHeader>
                    <ModalBody>
                        <NotificationEditor notification={canRead ? this.state.notifications[this.state.readIndex] : null} updateList={this.updateList} />
                    </ModalBody>
                    <ModalFooter><span className="float-right">SYSU MSC Push Services</span></ModalFooter>
                </Modal>

                <Modal isOpen={this.state.showSendingModal} toggle={this.togglesending}>
                    <ModalHeader toggle={this.togglesending}>发送通知邮件/短信</ModalHeader>
                    <ModalBody>
                        <p>将会向以下成员发送新消息通知：</p>
                        {
                            (canRead && this.state.showSendingModal && this.state.pushUsers !== null && this.state.pushUsers.succeeded) ?
                                <div style={{ maxHeight: '400px', overflow: 'auto' }}>
                                    <ListGroup>
                                        {this.state.pushUsers.users.map(x => (
                                            <ListGroupItem className="justify-content-between">
                                                <p>{x.name} {x.sexual === 1 ? "♂" : "♀"} {
                                                    x.department === 1 ? "行政策划部" :
                                                        x.department === 2 ? "媒体宣传部" :
                                                            x.department === 3 ? "综合技术部" : "暂无部门"
                                                }</p>
                                                <p>Id: {x.id} <a href={"/Account/Identity?userId=" + x.id} target="_blank">查看详情</a></p>
                                                <p>{x.email} {x.emailConfirmed ? "√" : "×"} {x.phoneNumber} {x.phoneNumberConfirmed ? "√" : "×"}</p>
                                            </ListGroupItem>
                                        ))}
                                    </ListGroup>
                                </div> : <p>加载中...</p>
                        }
                        <FormGroup>
                            <Label>发送方式</Label>&nbsp;
                            <div className="form-check-inline">
                                <label className="form-check-label">
                                    <Input type="checkbox" id="sendemail" />邮件
                                </label>
                            </div>
                            <div className="form-check-inline">
                                <label className="form-check-label">
                                    <Input type="checkbox" id="sendsms" />短信
                                </label>
                            </div>
                            <div className="form-check-inline">
                                <label className="form-check-label">
                                    <Input type="checkbox" id="requireresponse" />要求短信回执
                                </label>
                            </div>

                        </FormGroup>
                        <p>
                            {this.state.pushUsers !== null && this.state.pushUsers.users !== null ? <span>发送进度：{this.state.currentSent}/{this.state.pushUsers.users.length}</span> : null}
                            <Button color="primary" id="sendBtn" className="float-right" onClick={canRead ? () => this.sendEmailSMS(this.state.notifications[this.state.readIndex].id) : null}>发送</Button>
                        </p>
                        <div style={{ maxHeight: '200px', overflow: 'auto' }}>
                            <ListGroup id="errorlist" />
                        </div>
                    </ModalBody>
                    <ModalFooter><span className="float-right">SYSU MSC Push Services</span></ModalFooter>
                </Modal >
            </Container >
        );
    }
}