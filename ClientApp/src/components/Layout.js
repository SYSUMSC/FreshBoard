import React, { Component } from 'react';
import { NavMenu } from './NavMenu';
import { Nav, Navbar, NavLink } from 'reactstrap';

export class Layout extends Component {
    displayName = Layout.name

    render() {
        return (
            <div>
                <NavMenu user={this.props.user} />
                <div>
                    {this.props.children}
                </div>
                <footer>
                    <br />
                    <Navbar color="light" light>
                        <Nav className="ml-auto">
                            <NavLink className="float-left">SYSU MSC 2018</NavLink>
                        </Nav>
                    </Navbar>
                </footer>
            </div>
        );
    }
}
