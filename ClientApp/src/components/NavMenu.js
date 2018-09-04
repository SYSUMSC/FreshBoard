import React, { Component } from 'react';
import { NavLink } from 'react-router-dom';
import { Collapse, Navbar, NavbarToggler, Nav, NavItem, Form } from 'reactstrap';

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
                        <NavLink activeClassName="active" to={'/Account/Portal'} className="nav-link">Hi, {this.props.user.userInfo.name}!</NavLink>
                    </NavItem>
                    <NavItem>
                        <Form method="post" action="/Account/LogoutAsync">
                            <button className="btn btn-link nav-link">退出</button>
                        </Form>
                    </NavItem>
                </Nav>
                : null;

        return (
            <Navbar color="dark" dark expand="md">
                <NavLink className="navbar-brand" to={'/'}>
                    <span>SYSU 微软学生俱乐部</span>
                </NavLink>
                <NavbarToggler onClick={this.toggle} />
                <Collapse isOpen={this.state.isOpen} navbar>
                    <Nav navbar>
                        <NavItem>
                            <NavLink activeClassName="active" exact to={'/'} className="nav-link">主页</NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink activeClassName="active" to={'/Notification'} className="nav-link">通知</NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink activeClassName="active" to={'/Blogs'} className="nav-link">干货</NavLink>
                        </NavItem>
                    </Nav>
                    {accountPortal}
                </Collapse>
            </Navbar>
        );
    }
}
