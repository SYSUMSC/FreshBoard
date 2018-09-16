import React, { Component } from 'react';
import * as Dom from 'react-router-dom';
import { Collapse, Navbar, NavbarToggler, Nav, NavItem, Form, NavLink } from 'reactstrap';


export class NavMenu extends Component {
    displayName = NavMenu.name
    constructor(props) {
        super(props);

        this.toggle = this.toggle.bind(this);

        this.state = {
            isOpen: false
        };
    }

    toggle() {
        this.setState({
            isOpen: !this.state.isOpen
        });
    }

    render() {
        let accountPortal = this.props.user === null ? null :
            this.props.user.isSignedIn ?
                <Nav navbar className="ml-auto">
                    <NavItem>
                        <Dom.NavLink activeClassName="active" to={'/Account/Portal'} className="nav-link">Hi, {this.props.user.userInfo.name}!</Dom.NavLink>
                    </NavItem>
                    <NavItem>
                        <Form method="post" action="/Account/LogoutAsync">
                            <button className="btn btn-link nav-link">退出</button>
                        </Form>
                    </NavItem>
                </Nav>
                : <Nav navbar className="ml-auto">
                    <NavItem>
                        <NavLink href="javascript:void(0)" onClick={() => this.props.toggleLogin()} className="nav-link">注册/登录</NavLink>
                    </NavItem>
                </Nav>;

        return (
            <div>
                <Navbar color="dark" dark expand="md" fixed="top">
                    <Dom.NavLink className="navbar-brand" to={'/'}>
                        <span><img src="/favicon.ico" height="28" width="28" alt="SYSU MSC" /> SYSU 微软学生俱乐部</span>
                    </Dom.NavLink>
                    <NavbarToggler onClick={this.toggle} />
                    <Collapse isOpen={this.state.isOpen} navbar>
                        <Nav navbar>
                            <NavItem>
                                <Dom.NavLink activeClassName="active" exact to={'/'} className="nav-link">主页</Dom.NavLink>
                            </NavItem>
                            <NavItem>
                                <Dom.NavLink activeClassName="active" to={'/Notification'} className="nav-link">通知</Dom.NavLink>
                            </NavItem>
                            <NavItem>
                                <Dom.NavLink activeClassName="active" to={'/Blogs'} className="nav-link">干货</Dom.NavLink>
                            </NavItem>
                            <NavItem>
                                <Dom.NavLink activeClassName="active" to={'/Crack'} className="nav-link">解谜</Dom.NavLink>
                            </NavItem>
                        </Nav>
                        {accountPortal}
                    </Collapse>
                </Navbar>
            </div>
        );
    }
}
