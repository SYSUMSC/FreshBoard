import React, { Component } from "react";
import { Container, Button, Modal, ModalHeader, ModalBody, ModalFooter } from "reactstrap";
import { Post } from '../../utils/HttpRequest';
import { Modify } from "./Modify";
import { Apply } from "./Apply";
import QRCode from 'qrcode';
import { ModifyOther } from "./ModifyOther";
import { ConfirmPhone } from "./ConfirmPhone";

export class Portal extends Component {
    static ApplyStatus(userInfo) {
        return (
            <div>
                <table className='table'>
                    <thead>
                        <tr>
                            <th>部门</th>
                            <th>状态</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>{userInfo.department === 1 ? '行政策划部'
                                : userInfo.department === 2 ? '媒体宣传部'
                                    : userInfo.department === 3 ? '综合技术部'
                                        : '暂无'}
                            </td>
                            <td>{userInfo.applyStatus === 1 ? '等待一面'
                                : userInfo.applyStatus === 2 ? '等待二面'
                                    : userInfo.applyStatus === 3 ? '录取失败'
                                        : userInfo.applyStatus === 4 ? '录取成功'
                                            : '暂无'}</td>
                        </tr>
                    </tbody>
                </table>
                <br />
                <p>解谜进度：{userInfo.crackProgress}</p>
            </div>
        );
    }

    static SendingEmail = false

    static SendConfirmEmail() {
        if (Portal.SendingEmail) return;
        Portal.SendingEmail = true;
        Post('/Account/SendRegisterEmailAsync', {}, {})
            .then(res => res.json())
            .then(data => {
                Portal.SendingEmail = false;
                if (data.succeeded) alert('已发送一封关于验证邮箱的邮件到您的邮箱，请根据邮件说明验证邮箱');
                else alert(data.message);
            })
            .catch(() => {
                Portal.SendingEmail = false;
                alert('发生未知错误');
            });
    }

    static OtherInfoList(otherInfo) {
        if (otherInfo === null) return null;
        return (
            otherInfo.map(x => (<div>
                <strong>{x.description}</strong>
                <textarea className="form-control" readOnly>{x.value}</textarea>
                <br />
            </div>))
        );
    }

    displayName = Portal.name
    constructor(props) {
        super(props);
        this.toggleModal = this.toggleModal.bind(this);
        this.sendConfirmSMS = this.sendConfirmSMS.bind(this);
        this.userInfoList = this.userInfoList.bind(this);
        this.state = {
            isModalOpen: false,
            modalType: 0
        };
    }

    sendConfirmSMS() {
        this.setState({
            isModalOpen: true,
            modalType: 4
        });
    }

    userInfoList(userInfo) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>姓名</th>
                        <th>邮箱</th>
                        <th>生日</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.name} {userInfo.sexual === 1 ? '♂' : '♀'}</td>
                        <td>{userInfo.email} ({userInfo.emailConfirmed ? <span>已验证</span> : <a title="点击重新发送验证邮件" href="javascript:void(0)" onClick={Portal.SendConfirmEmail}>点击验证</a>})</td>
                        <td>{userInfo.dob}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <th>学号</th>
                        <th>院系</th>
                        <th>政治面貌</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.schoolNumber}</td>
                        <td>{userInfo.grade} {userInfo.institute} {userInfo.major}</td>
                        <td>{userInfo.cpcLevel === 0 ? '群众'
                            : userInfo.cpcLevel === 1 ? '共青团员'
                                : userInfo.cpcLevel === 2 ? '共产党员'
                                    : userInfo.cpcLevel === 3 ? '中共预备党员'
                                        : userInfo.cpcLevel === 4 ? '无党派人士'
                                            : '其他'}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <th>手机</th>
                        <th>QQ</th>
                        <th>WeChat</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.phoneNumber} <span id='phoneConfirmStatus'>({userInfo.phoneNumberConfirmed ? <span>已验证</span> : <a title="验证手机号码" href="javascript:void(0)" onClick={() => this.toggleModal(4)}>点击验证</a>})</span></td>
                        <td>{userInfo.qq}</td>
                        <td>{userInfo.weChat}</td>
                    </tr>
                </tbody>
            </table>
        );
    }

    toggleModal(type = 0) {
        this.setState({ isModalOpen: !this.state.isModalOpen, modalType: type });
    }

    render() {
        let userInfo = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                this.userInfoList(this.props.user.userInfo) : <p>请登录账户</p>;

        let departmentInfo = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                Portal.ApplyStatus(this.props.user.userInfo) : <p>请登录账户</p>;

        let otherInfo = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                Portal.OtherInfoList(this.props.user.otherInfo) : <p>请登录账户</p>;

        let modal = this.state.modalType === 1 ? <Modify user={this.props.user} updateStatus={this.props.updateStatus} closeModal={this.toggleModal} />
            : this.state.modalType === 2 ? <ModifyOther user={this.props.user} updateStatus={this.props.updateStatus} closeModal={this.toggleModal} />
                : this.state.modalType === 3 ? <Apply user={this.props.user} updateStatus={this.props.updateStatus} closeModal={this.toggleModal} />
                    : this.state.modalType === 4 ? <ConfirmPhone user={this.props.user} updateStatus={this.props.updateStatus} /> : null;

        this.props.user === null ? null :
            this.props.user.isSignedIn ?
                QRCode.toDataURL(window.location.protocol + '//' + window.location.host + '/Account/Identity?userId=' + this.props.user.userInfo.id)
                    .then(url => {
                        document.getElementById('userQR').src = url;
                    })
                    .catch(err => {
                        console.error(err);
                    }) : null;

        return (
            <Container>
                <br />
                <h2>我的账户</h2>
                <h4>基本信息 <Button color="primary" onClick={() => this.toggleModal(1)}>修改</Button></h4>
                {userInfo}
                <hr />
                <h4>部门申请 <Button color="primary" onClick={() => this.toggleModal(3)}>修改</Button></h4>
                {departmentInfo}
                <hr />
                <h4>其他信息 <Button color="primary" onClick={() => this.toggleModal(2)}>修改</Button></h4>
                {otherInfo}
                <hr />
                <h4>个人二维码</h4>
                <img id='userQR' alt='' src='' />
                <p className="text-danger">面试时请向面试官出示此二维码</p>

                <Modal isOpen={this.state.isModalOpen} toggle={this.toggleModal}>
                    <ModalHeader toggle={this.toggleModal}>修改信息</ModalHeader>
                    <ModalBody>
                        {modal}
                    </ModalBody>
                    <ModalFooter>
                        <p>SYSU MSC Account</p>
                    </ModalFooter>
                </Modal>

            </Container>
        );
    }
}