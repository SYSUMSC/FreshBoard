import React, { Component } from "react";
import { Container, Button, Modal, ModalHeader, ModalBody, ModalFooter } from "reactstrap";
import { Post } from '../../utils/HttpRequest';
import { Modify } from "./Modify";
import { Apply } from "./Apply";

export class Portal extends Component {
    static ApplyStatus(userInfo) {
        return (
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
                        <td>{userInfo.applyStatus === 1 ? '等待第一次面试'
                            : userInfo.applyStatus === 2 ? '等待第二次面试'
                                : userInfo.applyStatus === 3 ? '录取失败'
                                    : userInfo.applyStatus === 4 ? '录取成功'
                                        : '暂无'}</td>
                    </tr>
                </tbody>
            </table>
        );
    }

    static SendConfirmEmail() {
        Post('/Account/SendRegisterEmailAsync', {}, {})
            .then(res => res.json())
            .then(data => {
                if (data.succeeded) alert('发送成功');
                else alert(data.message);
            })
            .catch(() => alert('发送失败'));
    }

    static UserInfoList(userInfo) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>姓名</th>
                        <th>邮箱</th>
                        <th>学号</th>
                        <th>院系</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.name} {userInfo.sexual === 1 ? '♂' : '♀'}</td>
                        <td>{userInfo.email} ({userInfo.emailConfirmed ? <span>已验证</span> : <a title="点击重新发送验证邮件" href="javascript:void(0)" onClick={Portal.SendConfirmEmail}>未验证</a>})</td>
                        <td>{userInfo.schoolNumber}</td>
                        <td>{userInfo.grade} {userInfo.institute} {userInfo.major}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <th>电话</th>
                        <th>QQ</th>
                        <th>WeChat</th>
                        <th>政治面貌</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.phoneNumber}</td>
                        <td>{userInfo.qq}</td>
                        <td>{userInfo.weChat}</td>
                        <td>{userInfo.cpcLevel === 0 ? '群众'
                            : userInfo.cpcLevel === 1 ? '共青团员'
                                : userInfo.cpcLevel === 2 ? '共产党员'
                                    : userInfo.cpcLevel === 3 ? '中共预备党员'
                                        : userInfo.cpcLevel === 4 ? '无党派人士'
                                            : '其他'}</td>
                    </tr>
                </tbody>
            </table>
        );
    }

    displayName = Portal.name
    constructor(props) {
        super(props);
        this.toggleModifyModal = this.toggleModifyModal.bind(this);
        this.toggleApplyModal = this.toggleApplyModal.bind(this);
        this.state = {
            modifyModalOpen: false,
            applyModalOpen: false
        };
    }

    toggleModifyModal() {
        this.setState({ modifyModalOpen: !this.state.modifyModalOpen });
    }

    toggleApplyModal() {
        this.setState({ applyModalOpen: !this.state.applyModalOpen });
    }

    render() {
        let userInfo = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                Portal.UserInfoList(this.props.user.userInfo) : <p>没有数据</p>;

        let departmentInfo = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                Portal.ApplyStatus(this.props.user.userInfo) : <p>没有数据</p>;

        return (
            <Container>
                <br />
                <h2>我的账户</h2>
                <h4>个人信息 <Button color="primary" onClick={this.toggleModifyModal}>修改</Button></h4>
                {userInfo}
                <hr />
                <h4>部门申请 <Button color="primary" onClick={this.toggleApplyModal}>修改</Button></h4>
                {departmentInfo}

                <Modal isOpen={this.state.modifyModalOpen} toggle={this.toggleModifyModal}>
                    <ModalHeader toggle={this.toggleModifyModal}>修改信息</ModalHeader>
                    <ModalBody>
                        <Modify user={this.props.user} />
                    </ModalBody>
                    <ModalFooter>
                        <p>SYSU MSC Account</p>
                    </ModalFooter>
                </Modal>
                
                <Modal isOpen={this.state.applyModalOpen} toggle={this.toggleApplyModal}>
                    <ModalHeader toggle={this.toggleApplyModal}>修改信息</ModalHeader>
                    <ModalBody>
                        <Apply user={this.props.user} />
                    </ModalBody>
                    <ModalFooter>
                        <p>SYSU MSC Account</p>
                    </ModalFooter>
                </Modal>
            </Container>
        );
    }
}