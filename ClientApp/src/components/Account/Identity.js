import React, { Component } from "react";
import { Container } from "reactstrap";

export class Identity extends Component {
    static UserInfoList(userInfo) {
        if (userInfo === null) return <p>没有数据</p>;
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>姓名</th>
                        <th>邮箱</th>
                        <th>生日</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.name} {userInfo.sexual === 1 ? '♂' : '♀'}</td>
                        <td>{userInfo.email} ({userInfo.emailConfirmed ? <span>已验证</span> : <span>未验证</span>})</td>
                        <td>{userInfo.dob}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <th>学号</th>
                        <th>院系</th>
                        <th>政治面貌</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.schoolNumber}</td>
                        <td>{userInfo.grade} {userInfo.institute} {userInfo.major}</td>
                        <td>{userInfo.cpcLevel === 0 ? '群众'
                            : userInfo.cpcLevel === 1 ? '共青团员'
                                : userInfo.cpcLevel === 2 ? '共产党员'
                                    : userInfo.cpcLevel === 3 ? '中共预备党员'
                                        : userInfo.cpcLevel === 4 ? '无党派人士'
                                            : '其他'}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <th>手机</th>
                        <th>QQ</th>
                        <th>WeChat</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{userInfo.phoneNumber} ({userInfo.phoneNumberConfirmed ? <span>已验证</span> : <span>未验证</span>})</td>
                        <td>{userInfo.qq}</td>
                        <td>{userInfo.weChat}</td>
                    </tr>
                </tbody>
            </table>
        );
    }

    static OtherInfoList(otherInfo) {
        if (otherInfo === null) return null;
        return (
            otherInfo.map(x => (<div>
                <strong>{x.description}</strong>
                <textarea className="form-control" readOnly>{x.value}</textarea>
                <br />
            </div>))
        );
    }

    static ApplyStatus(userInfo) {
        if (userInfo === null) return <p>没有数据</p>;
        return (
            <div>
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
                            <td>{userInfo.applyStatus === 1 ? '等待一面'
                                : userInfo.applyStatus === 2 ? '等待二面'
                                    : userInfo.applyStatus === 3 ? '录取失败'
                                        : userInfo.applyStatus === 4 ? '录取成功'
                                            : '暂无'}</td>
                        </tr>
                    </tbody>
                </table>
                <br />
                <p>解谜进度：{userInfo.crackProgress}</p>
            </div>
        );
    }

    displayName = Identity.name
    constructor(props) {
        super(props);
        this.state = {
            userInfo: null,
            otherInfo: null,
            loading: true
        };
        const p = window.location.toString();
        const param = p.substring(p.indexOf('?'));
        fetch('/Account/GetSpecificUserInfoAsync' + param, { method: 'GET', credentials: "same-origin" })
            .then(res => res.json())
            .then(data => {
                if (data.succeeded) {
                    this.setState({
                        userInfo: data.userInfo,
                        otherInfo: data.otherInfo,
                        loading: false
                    });
                }
                else {
                    alert(data.message);
                    this.setState({ loading: false });
                }
            }).catch(() => {
                alert('发生未知错误');
                this.setState({ loading: false });
            });
    }



    render() {
        let userInfo = this.state.loading ? <p>加载中...</p> :
            this.state.userInfo !== null ? Identity.UserInfoList(this.state.userInfo) : <p>没有数据</p>;

        let departmentInfo = this.state.loading ? <p>加载中...</p> :
            this.state.userInfo !== null ? Identity.ApplyStatus(this.state.userInfo) : <p>没有数据</p>;

        let otherInfo = this.state.loading ? <p>加载中...</p> :
            this.state.otherInfo !== null ? Identity.OtherInfoList(this.state.otherInfo) : <p>没有数据</p>;

        return (
            <Container>
                <br />
                <h2>成员信息</h2>
                <h4>基本信息</h4>
                {userInfo}
                <hr />
                <h4>其他信息</h4>
                {otherInfo}
                <hr />
                <h4>部门申请</h4>
                {departmentInfo}
            </Container>
        );
    }
}