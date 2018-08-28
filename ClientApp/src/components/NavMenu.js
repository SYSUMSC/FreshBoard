import React, { Component } from 'react';
import { Link } from 'react-router-dom';
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
            <Navbar color="dark" dark expand="md" collapseOnSelect>
                <NavbarBrand>
                    <Link className="navbar-brand" to={'/'}>
                        <span>SYSU 微软学生俱乐部</span>
                    </Link>
                </NavbarBrand>
                <NavbarToggler onClick={this.toggle} />
                <Collapse isOpen={this.state.isOpen} navbar>
                    <Nav navbar>
                        <NavItem>
                            <Link to={'/'}>
                                <NavLink>主页</NavLink>
                            </Link>
                        </NavItem>
                        <NavItem>
                            <Link to={'/counter'}>
                                <NavLink>Counter</NavLink>
                            </Link>
                        </NavItem>
                        <NavItem>
                            <Link to={'/fetchdata'}>
                                <NavLink>Fetch Data</NavLink>
                            </Link>
                        </NavItem>
                    </Nav>
                </Collapse>
            </Navbar>
        );
    }
}
