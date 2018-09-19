import React, { Component } from "react";
import { Container, Button } from "reactstrap";
import { NavLink } from 'react-router-dom';

export class AdminIndex extends Component {
    displayName = AdminIndex.name

    constructor(props) {
        super(props);
        this.adminPanel = this.adminPanel.bind(this);
    }

    adminPanel() {
        return (
            <div>
                <NavLink to={'/Account/Admin/NotificationManager'}><Button color="primary">通知管理</Button></NavLink>
                &nbsp;
                <NavLink to={'/Account/Admin/ApplyManager'}><Button color="primary">申请管理</Button></NavLink>
                &nbsp;
                <NavLink to={'/Account/Admin/PrivilegeManager'}><Button color="primary">权限管理</Button></NavLink>
                &nbsp;
                <NavLink to={'/Account/Admin/ProblemManager'}><Button color="primary">题目管理</Button></NavLink>
                &nbsp;
                <NavLink to={'/Account/Admin/SearchUsers'}><Button color="primary">浏览用户</Button></NavLink>
            </div>
        );
    }

    render() {
        let panel = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                (this.props.user.userInfo.privilege !== 1 ? <p>没有权限</p>
                    : this.adminPanel()) : <p>请登录</p>;

        return (
            <Container>
                <br />
                <h2>管理后台</h2>
                {panel}
            </Container>
        );
    }
}