import React, { Component } from 'react';
import { NavMenu } from './NavMenu';
import { Nav, Navbar, NavLink } from 'reactstrap';

export class Layout extends Component {
    displayName = Layout.name

    render() {
        return (
            <div>
                <NavMenu user={this.props.user} />
                <div id="body" className="body-content">
                    {this.props.children}
                </div>
                <footer id="footer">
                    <br />
                    <Navbar color="light" fixed="bottom" light>
                        <Nav className="ml-auto">
                            <NavLink className="float-left">SYSU MSC 2018</NavLink>
                        </Nav>
                    </Navbar>
                </footer>
            </div>
        );
    }
}
