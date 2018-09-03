import React, { Component } from 'react';
import { Login } from './Account/Login';
import { Container } from 'reactstrap';

export class Notification extends Component {
    displayName = Notification.name;

    constructor(props) {
        super(props);
    }

    render() {
        let panel =
            this.props.user === null ? <p>没有通知</p> :
                this.props.user.isSignedIn ?
                    <p>没有通知</p>
                    :
                    <div><h4>请先登录</h4> <hr /> <Login /><br /></div>;

        return (
            <Container>
                <br />
                {panel}
            </Container>
        );
    }
}