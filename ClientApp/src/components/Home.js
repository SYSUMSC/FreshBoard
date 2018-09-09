import React, { Component } from 'react';
import * as Dom from 'react-router-dom';
import { Jumbotron, TabPane, Button, Container, Modal, ModalBody, ModalHeader, ModalFooter, Nav, NavItem, NavLink, TabContent, Row, Col } from 'reactstrap';
import classnames from 'classnames';
import { Register } from './Account/Register';
import { Login } from './Account/Login';

export class Home extends Component {
    displayName = Home.name
    constructor(props) {
        super(props);

        this.toggleLogin = this.toggleLogin.bind(this);
        this.state = {
            loginShown: false,
            activeTab: '1'
        };

        this.switchTab = this.switchTab.bind(this);

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

    render() {
        let adminPortal = this.props.user === null ? null :
            this.props.user.isSignedIn ?
                (this.props.user.userInfo.privilege !== 1 ? null
                    : <Dom.NavLink to={'/Account/Admin/Index'}>
                        <Button color="primary">进入管理后台</Button>
                    </Dom.NavLink>) : null;

        let loginPortal = this.props.user === null ? <p>登录中...</p> :
            this.props.user.isSignedIn
                ?
                <div>
                    <Dom.NavLink to={'/Account/Portal'}>
                        <Button color="primary">进入账户</Button>
                    </Dom.NavLink>&nbsp;
                    {adminPortal}
                </div>
                : <Button color="primary" onClick={this.toggleLogin}>立即上车</Button>;
        return (
            <div className="header">
                <Container>
                    <div className="intro-text">
                        <h1>中山大学<span style={{ color: '#28ABE3' }}>微软学生俱乐部</span></h1>
                        <p>船新的 MSC 等你来加入~</p>
                        {loginPortal}
                        <br />
                        <br />
                        <small className="text-info">加入方法：立即上车 -- 注册/登录账号 -- 进入账户 -- 补全相关信息 -- 申请部门，so easy~</small>
                    </div>
                </Container>

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
