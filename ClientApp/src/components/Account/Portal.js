import React, { Component } from "react";
import { Container } from "reactstrap";

export class Portal extends Component {
    static ApplyStatus(userInfo) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>部门</th>
                        <th>状态</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.department === 1 ? '行政策划部'
                            : userInfo.department === 2 ? '媒体宣传部'
                                : userInfo.department === 3 ? '综合技术部'
                                    : '暂无'}
                        </td>
                        <td>{userInfo.applyStatus === 1 ? '等待第一次面试'
                            : userInfo.applyStatus === 2 ? '等待第二次面试'
                                : userInfo.applyStatus === 3 ? '录取失败'
                                    : userInfo.applyStatus === 4 ? '录取成功'
                                        : '暂无'}</td>
                    </tr>
                </tbody>
            </table>
        );
    }

    static UserInfoList(userInfo) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>姓名</th>
                        <th>邮箱</th>
                        <th>学号</th>
                        <th>院系</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.name} {userInfo.sexual === 1 ? '♂' : '♀'}</td>
                        <td>{userInfo.email}</td>
                        <td>{userInfo.schoolNumber}</td>
                        <td>{userInfo.grade} {userInfo.institute} {userInfo.major}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <th>电话</th>
                        <th>QQ</th>
                        <th>WeChat</th>
                        <th>政治面貌</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.phoneNumber}</td>
                        <td>{userInfo.qq}</td>
                        <td>{userInfo.weChat}</td>
                        <td>{userInfo.cPCLevel === 0 ? '群众'
                            : userInfo.cPCLevel === 1 ? '共青团员'
                                : userInfo.cPCLevel === 2 ? '共产党员'
                                    : userInfo.cPCLevel === 3 ? '中共预备党员'
                                        : userInfo.cPCLevel === 4 ? '无党派人士'
                                            : '其他'}</td>
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
        let userInfo = this.props.user === null ? <em>加载中...</em> :
            this.props.user.isSignedIn ?
                Portal.UserInfoList(this.props.user.userInfo) : <em>没有数据</em>;

        let departmentInfo = this.props.user === null ? <em>加载中...</em> :
            this.props.user.isSignedIn ?
                Portal.ApplyStatus(this.props.user.userInfo) : <em>没有数据</em>;

        return (
            <Container>
                <h1>我的账户</h1>
                <h4>个人信息</h4>
                {userInfo}
                <hr />
                <h4>部门申请</h4>
                {departmentInfo}
            </Container>
        );
    }
}