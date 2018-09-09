import React, { Component } from 'react';
import { NavMenu } from './NavMenu';
import { Nav, Navbar, NavLink } from 'reactstrap';

export class Layout extends Component {
    displayName = Layout.name

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        var num = 10;
        var obj = document.getElementById('preloader');
        console.log(obj);
        var st = setInterval(function () {
            num--;
            obj.style.opacity = num / 10;
            if (num <= 0) {
                obj.remove();
                clearInterval(st);
            }
        }, 30);
    }

    render() {
        return (
            <div>
                <NavMenu user={this.props.user} />
                <div className="body-content">
                    {this.props.children}
                </div>
                <footer>
                    <br />
                    <Navbar className="fixed-bottom" color="light" fixed light>
                        <Nav className="ml-auto">
                            <NavLink className="float-left">SYSU MSC 2018</NavLink>
                        </Nav>
                    </Navbar>
                </footer>
            </div>
        );
    }
}
