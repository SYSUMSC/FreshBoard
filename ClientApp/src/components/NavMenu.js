import React, { Component } from 'react';
import * as Dom from 'react-router-dom';
import { Collapse, Navbar, NavbarToggler, NavbarBrand, Nav, NavItem, NavLink } from 'reactstrap';

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
        let accountPortal = this.props.user == null ? null :
            this.props.user.isSignedIn ?
                <Nav navbar className="ml-auto">
                    <NavItem>
                        <Dom.NavLink activeClassName="active" to={'/portal'}>
                            <NavLink>Hi, {this.props.user.userInfo.userName}!</NavLink>
                        </Dom.NavLink>
                    </NavItem>
                    <NavItem>
                        <form method="post" action="/Account/LogoutAsync">
                            <button className="btn btn-link nav-link">退出</button>
                        </form>
                    </NavItem>
                </Nav>
                : null;

        return (
            <Navbar color="dark" dark expand="md">
                <NavbarBrand>
                    <Dom.Link className="navbar-brand" to={'/'}>
                        <span>SYSU 微软学生俱乐部</span>
                    </Dom.Link>
                </NavbarBrand>
                <NavbarToggler onClick={this.toggle} />
                <Collapse isOpen={this.state.isOpen} navbar>
                    <Nav navbar>
                        <NavItem>
                            <Dom.NavLink activeClassName="active" exact to={'/'}>
                                <NavLink>主页</NavLink>
                            </Dom.NavLink>
                        </NavItem>
                        <NavItem>
                            <Dom.NavLink activeClassName="active" to={'/Notification'}>
                                <NavLink>通知</NavLink>
                            </Dom.NavLink>
                        </NavItem>
                        <NavItem>
                            <Dom.NavLink activeClassName="active" to={'/Blogs'}>
                                <NavLink>干货</NavLink>
                            </Dom.NavLink>
                        </NavItem>
                    </Nav>
                    {accountPortal}
                </Collapse>
            </Navbar>
        );
    }
}
