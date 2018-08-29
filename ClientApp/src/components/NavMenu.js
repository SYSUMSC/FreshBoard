﻿import React, { Component } from 'react';
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
                            <Dom.NavLink activeClassName="active" to={'/counter'}>
                                <NavLink>Counter</NavLink>
                            </Dom.NavLink>
                        </NavItem>
                        <NavItem>
                            <Dom.NavLink activeClassName="active" to={'/fetchdata'}>
                                <NavLink>Fetch Data</NavLink>
                            </Dom.NavLink>
                        </NavItem>
                    </Nav>
                </Collapse>
            </Navbar>
        );
    }
}
