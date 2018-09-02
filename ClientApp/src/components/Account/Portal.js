import React, { Component } from "react";
import { Container } from "reactstrap";

export class Portal extends Component {
    static UserInfoList(userInfo) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>姓名</th>
                        <th>Email</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.name}</td>
                        <td>{userInfo.email}</td>
                    </tr>
                </tbody>
            </table>
        );
    }

    displayName = Portal.name
    constructor(props) {
        super(props);
    }

    render() {
        let userInfo = this.props.user === null ? <em>Loading...</em> :
            this.props.user.isSignedIn ?
                Portal.UserInfoList(this.props.user.userInfo) : <em>No data</em>;
        return (
            <Container>
                <h1>账户信息</h1>
                {userInfo}
            </Container>
        );
    }
}