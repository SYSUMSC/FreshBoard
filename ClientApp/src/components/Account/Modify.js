import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input } from "reactstrap";
import { FormPost } from "../../utils/HttpRequest";

export class Modify extends Component {
    displayName = Modify.name;

    constructor(props) {
        super(props);
        this.modify = this.modify.bind(this);
        this.fillinDefaultInfo = this.fillinDefaultInfo.bind(this);

        this.state = {
            disableButton: false
        };
    }

    fillinDefaultInfo(userInfo) {
        return (
            <Form id='modifyForm'>
                <FormGroup>
                    <Label for="name">姓名</Label>
                    <Input type="text" name="name" id="name" minLength="2" maxLength="15" required defaultValue={userInfo.name} />
                </FormGroup>

                <FormGroup>
                    <Label for="email">电子邮箱</Label>
                    <Input type="email" name="email" id="email" required defaultValue={userInfo.email} />
                </FormGroup>

                <FormGroup>
                    <Label for="dob">出生日期</Label>
                    <Input type="date" name="dob" id="dob" required defaultValue={userInfo.dob} />
                </FormGroup>

                <FormGroup>
                    <Label for="grade">年级</Label>
                    <Input type="number" name="grade" id="grade" min="2000" max="2018" required defaultValue={userInfo.grade} />
                </FormGroup>

                <FormGroup>
                    <Label for="phone">电话</Label>
                    <Input type="text" name="phone" id="phone" required defaultValue={userInfo.phoneNumber} />
                </FormGroup>

                <FormGroup>
                    <Label for="wechat">WeChat</Label>
                    <Input type="text" name="wechat" id="wechat" required defaultValue={userInfo.weChat} />
                </FormGroup>

                <FormGroup>
                    <Label for="qq">QQ</Label>
                    <Input type="text" name="qq" id="qq" maxLength="12" required defaultValue={userInfo.qq} />
                </FormGroup>

                <FormGroup>
                    <Label for="sexual">性别</Label>&nbsp;
                        <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="sexual" id="sexual1" value="1" defaultChecked={userInfo.sexual === 1} />男
                            </label>
                    </div>

                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="sexual" id="sexual2" value="2" defaultChecked={userInfo.sexual === 2} />女
                            </label>
                    </div>
                </FormGroup>

                <FormGroup>
                    <Label for="cpclevel">政治面貌</Label>&nbsp;
                        <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="cpclevel" id="cpclevel0" value="0" defaultChecked={userInfo.cpcLevel === 0} />群众
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="cpclevel" id="cpclevel1" value="1" defaultChecked={userInfo.cpcLevel === 1} />共青团员
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="cpclevel" id="cpclevel2" value="2" defaultChecked={userInfo.cpcLevel === 2} />共产党员
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="cpclevel" id="cpclevel3" value="3" defaultChecked={userInfo.cpcLevel === 3} />中共预备党员
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="cpclevel" id="cpclevel4" value="4" defaultChecked={userInfo.cpcLevel === 4} />无党派人士
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="cpclevel" id="cpclevel5" value="5" defaultChecked={userInfo.cpcLevel === 5} />其他
                            </label>
                    </div>
                </FormGroup>

                <FormGroup>
                    <Label for="schnum">学号</Label>
                    <Input type="text" name="schnum" id="schnum" minLength="8" maxLength="8" required defaultValue={userInfo.schoolNumber} />
                </FormGroup>

                <FormGroup>
                    <Label for="institute">学院</Label>
                    <Input type="text" name="institute" id="institute" required defaultValue={userInfo.institute} />
                </FormGroup>

                <FormGroup>
                    <Label for="major">专业</Label>
                    <Input type="text" name="major" id="major" required defaultValue={userInfo.major} />
                </FormGroup>

                <Button type="button" className="float-right" color="primary" onClick={this.modify} disabled={this.state.disableButton}>修改</Button>
            </Form>);
    }

    modify() {
        var form = document.getElementById('modifyForm');
        if (form.reportValidity()) {
            this.setState({ disableButton: true });
            FormPost('/Account/ModifyAsync', form)
                .then(response => response.json())
                .then(data => {
                    if (data.succeeded) {
                        alert('修改成功');
                        window.location = '/Account/Portal';
                    }
                    else {
                        alert(data.message);
                        this.setState({ disableButton: false });
                    }
                })
                .catch(() => { alert('修改失败'); this.setState({ disableButton: false }); });
        }
    }

    render() {
        let modifyPanel = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                this.fillinDefaultInfo(this.props.user.userInfo) : <p>没有数据</p>;



        return (
            <div>
                {modifyPanel}
            </div>
        );
    }
}