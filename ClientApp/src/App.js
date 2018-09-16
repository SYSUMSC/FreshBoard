import React, { Component } from 'react';
import { Route, Switch } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Blogs } from './components/Blogs';
import { Notification } from './components/Notification';
import { Get } from './utils/HttpRequest';
import { Portal } from './components/Account/Portal';
import { ConfirmEmail } from './components/Account/ConfirmEmail';
import { Identity } from './components/Account/Identity';
import { ApplyManager } from './components/Account/Admin/ApplyManager';
import { NotificationManager } from './components/Account/Admin/NotificationManager';
import { AdminIndex } from './components/Account/Admin/Index';
import { PrivilegeManager } from './components/Account/Admin/PrivilegeManager';
import { Crack } from './components/Crack';
import { ProblemManager } from './components/Account/Admin/ProblemManager';
import { ResetPassword } from './components/Account/ResetPassword';
import { NotFound } from './components/NotFound';
import classnames from 'classnames';
import { Register } from './components/Account/Register';
import { Login } from './components/Account/Login';
import { TabPane, Modal, ModalBody, ModalHeader, ModalFooter, TabContent, Row, Col, Nav, NavLink, NavItem } from 'reactstrap';

export default class App extends Component {
    displayName = App.name

    constructor(props) {
        super(props);
        this.getUserStatus = this.getUserStatus.bind(this);
        this.toggleLogin = this.toggleLogin.bind(this);
        this.switchTab = this.switchTab.bind(this);
        this.state = {
            user: null,
            loginShown: false,
            activeTab: '1'
        };
        this.getUserStatus();
    }

    toggleLogin() {
        this.setState({
            loginShown: !this.state.loginShown
        });
    }

    switchTab(tab) {
        if (this.state.activeTab !== tab) {
            this.setState({
                activeTab: tab
            });
        }
    }

    getUserStatus() {
        Get('/Account/GetUserInfoAsync')
            .then(data => data.json())
            .then(data => {
                if (data.userInfo !== null) {
                    var dob = data.userInfo.dob.toString();
                    data.userInfo.dob = dob.substring(0, dob.indexOf('T'));
                }
                this.setState({ user: null });
                this.setState({ user: data });
            })
            .catch(() => alert('用户信息获取失败'));
    }

    setTitle(title) {
        document.title = title + " - SYSU MSC";
    }

    render() {
        return (
            <div>
                <Layout user={this.state.user} toggleLogin={this.toggleLogin}>
                    <Switch>
                        <Route exact path='/' render={() => { this.setTitle('主页'); return <Home user={this.state.user} toggleLogin={this.toggleLogin} />; }} />
                        <Route exact path='/Notification' render={() => { this.setTitle('通知'); return <Notification user={this.state.user} />; }} />
                        <Route exact path='/Blogs' render={() => { this.setTitle('干货'); return <Blogs user={this.state.user} />; }} />
                        <Route exact path='/Crack' render={() => { this.setTitle('解谜'); return <Crack user={this.state.user} updateStatus={this.getUserStatus} />; }} />
                        <Route exact path='/Account/Portal' render={() => { this.setTitle('我的账户'); return <Portal user={this.state.user} updateStatus={this.getUserStatus} />; }} />
                        <Route path='/Account/ConfirmEmail' render={() => { this.setTitle('验证邮箱'); return <ConfirmEmail user={this.state.user} updateStatus={this.getUserStatus} />; }} />
                        <Route path='/Account/ResetPassword' render={() => { this.setTitle('重置密码'); return <ResetPassword />; }} />
                        <Route path='/Account/Identity' render={() => { this.setTitle('成员信息'); return <Identity user={this.state.user} />; }} />
                        <Route path='/Account/Admin/Index' render={() => { this.setTitle('管理后台'); return <AdminIndex user={this.state.user} />; }} />
                        <Route path='/Account/Admin/NotificationManager' render={() => { this.setTitle('通知管理'); return <NotificationManager user={this.state.user} />; }} />
                        <Route path='/Account/Admin/ApplyManager' render={() => { this.setTitle('申请管理'); return <ApplyManager user={this.state.user} />; }} />
                        <Route path='/Account/Admin/PrivilegeManager' render={() => { this.setTitle('权限管理'); return <PrivilegeManager user={this.state.user} />; }} />
                        <Route path='/Account/Admin/ProblemManager' render={() => { this.setTitle('题目管理'); return <ProblemManager user={this.state.user} />; }} />
                        <Route render={() => { this.setTitle('找不到页面'); return <NotFound />; }} />
                    </Switch>
                </Layout>

                <Modal isOpen={this.state.loginShown} toggle={this.toggleLogin}>
                    <ModalHeader toggle={this.toggleLogin}>加入 MSC！</ModalHeader>
                    <ModalBody>
                        <p>注册 MSC Freshman 账号可以方便的申请入部、查看面试状态、查看录取结果以及收取相关通知</p>
                        <div>
                            <Nav tabs>
                                <NavItem>
                                    <NavLink
                                        className={classnames({ active: this.state.activeTab === '1' })}
                                        onClick={() => { this.switchTab('1'); }}
                                        href='#'
                                    >
                                        登录
                                </NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink
                                        className={classnames({ active: this.state.activeTab === '2' })}
                                        onClick={() => { this.switchTab('2'); }}
                                        href='#'
                                    >
                                        注册
                                </NavLink>
                                </NavItem>
                            </Nav>
                            <br />
                            <TabContent activeTab={this.state.activeTab}>
                                <TabPane tabId="1">
                                    <Row>
                                        <Col sm="12">
                                            <Login />
                                        </Col>
                                    </Row>
                                </TabPane>
                                <TabPane tabId="2">
                                    <Row>
                                        <Col sm="12">
                                            <Register />
                                        </Col>
                                    </Row>
                                </TabPane>
                            </TabContent>
                        </div>
                    </ModalBody>
                    <ModalFooter>
                        <p>SYSU MSC Account</p>
                    </ModalFooter>
                </Modal>
            </div>
        );
    }
}
