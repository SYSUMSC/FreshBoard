import React, { Component } from 'react';
import * as Dom from 'react-router-dom';
import { TabPane, Button, Container, Modal, ModalBody, ModalHeader, ModalFooter, Nav, NavItem, NavLink, TabContent, Row, Col } from 'reactstrap';
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

    componentDidMount() {
        var footer = document.getElementById('footer');
        if (footer !== null) footer.style = "display:none";
        var body = document.getElementById('body');
        if (body !== null) body.style = "margin-bottom: 0px";

    }

    componentWillUnmount() {
        var footer = document.getElementById('footer');
        if (footer !== null) footer.style = "";
        var body = document.getElementById('body');
        if (body !== null) body.style = "margin-bottom: 50px";
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

        let loginPortal = this.props.user === null ? <small>登录中...</small> :
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
            <div>
                <div className="header">
                    <div className="intro-text">
                        <h1>中山大学<span style={{ color: '#28ABE3' }}>微软学生俱乐部</span></h1>
                        <p>船新的 MSC 等你来加入~</p>
                        {loginPortal}
                        <br />
                        <br />
                        <small className="text-info">加入方法：立即上车 -- 注册/登录账号 -- 进入账户 -- 补全相关信息 -- 申请部门，so easy~</small>
                    </div>
                </div>
                <section id="about-us" className="about-us-section-1">
                    <Container>
                        <Row>
                            <Col md={12} sm={12}>
                                <div className="section-title text-center">
                                    <h2>关于我们</h2>
                                    <p>中山大学微软学生俱乐部各部门介绍</p>
                                </div>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={4} sm={4}>
                                <div className="welcome-section text-center waves-effect">
                                    <img className="img-responsive" src="/images/photo-1.jpg" alt=".." />
                                    <h4>行政策划部</h4>
                                    <div className="border" />
                                    <p>我们负责组织策划俱乐部的各种活动。无论是俱乐部举办的比赛，还是俱乐部成员一起吃喝玩乐的 Team Building，都由我们负责组织策划。</p>
                                </div>
                            </Col>

                            <Col md={4} sm={4}>
                                <div className="welcome-section text-center waves-effect">
                                    <img className="img-responsive" src="/images/photo-2.jpg" alt=".." />
                                    <h4>媒体宣传部</h4>
                                    <div className="border" />
                                    <p>我们的宗旨是：设计-技术多面手，社团-学术人气王。作为 MSC 的门面，推送、海报我们不在话下，煤船里面的大佬可不亚于隔壁技术部哦。</p>
                                </div>
                            </Col>

                            <Col md={4} sm={4}>
                                <div className="welcome-section text-center waves-effect">
                                    <img className="img-responsive" src="/images/photo-3.jpg" alt=".." />
                                    <h4>综合技术部</h4>
                                    <div className="border" />
                                    <p>无论你是希望在 iGEM、前后端开发、机器学习、编程语言理论等上摘金夺银，还是倾心于设计研发创造全新价值，MSC 都能给予你极大支持。</p>
                                </div>
                            </Col>

                        </Row>

                    </Container>
                </section>

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
