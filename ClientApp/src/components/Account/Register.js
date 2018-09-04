import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input } from "reactstrap";
import { FormPost } from "../../utils/HttpRequest";

export class Register extends Component {
    displayName = Register.name;

    constructor(props) {
        super(props);
        this.register = this.register.bind(this);

        this.state = {
            disableButton: false
        };
    }

    register() {
        var form = document.getElementById('registerForm');
        if (form.reportValidity()) {
            this.setState({ disableButton: true });
            FormPost('/Account/RegisterAsync', form)
                .then(response => response.json())
                .then(data => {
                    if (data.succeeded) {
                        alert('注册成功');
                        window.location = '/';
                    }
                    else {
                        alert(data.message);
                        this.setState({ disableButton: false });
                    }
                })
                .catch(() => { alert('注册失败'); this.setState({ disableButton: false }); });
        }
    }

    render() {
        return (
            <div>
                <Form id='registerForm'>
                    <FormGroup>
                        <Label for="name">姓名</Label>
                        <Input type="text" name="name" id="name" minLength="2" maxLength="15" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="email">电子邮箱</Label>
                        <Input type="email" name="email" id="email" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="grade">年级</Label>
                        <Input type="number" name="grade" id="grade" min="2000" max="2018" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="phone">电话</Label>
                        <Input type="text" name="phone" id="phone" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="wechat">WeChat</Label>
                        <Input type="text" name="wechat" id="wechat" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="qq">QQ</Label>
                        <Input type="text" name="qq" id="qq" maxLength="12" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="sexual">性别</Label>&nbsp;
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="sexual" id="sexual1" value="1" defaultChecked />男
                            </label>
                        </div>

                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="sexual" id="sexual2" value="2" />女
                            </label>
                        </div>
                    </FormGroup>

                    <FormGroup>
                        <Label for="cpclevel">政治面貌</Label>&nbsp;
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="cpclevel" id="cpclevel0" value="0" defaultChecked />群众
                            </label>
                        </div>
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="cpclevel" id="cpclevel1" value="1" />共青团员
                            </label>
                        </div>
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="cpclevel" id="cpclevel2" value="2" />共产党员
                            </label>
                        </div>
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="cpclevel" id="cpclevel3" value="3" />中共预备党员
                            </label>
                        </div>
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="cpclevel" id="cpclevel4" value="4" />无党派人士
                            </label>
                        </div>
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="cpclevel" id="cpclevel5" value="5" />其他
                            </label>
                        </div>
                    </FormGroup>

                    <FormGroup>
                        <Label for="schnum">学号</Label>
                        <Input type="text" name="schnum" id="schnum" minLength="8" maxLength="8" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="institute">学院</Label>
                        <Input type="text" name="institute" id="institute" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="major">专业</Label>
                        <Input type="text" name="major" id="major" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="confirmpassword">确认密码</Label>
                        <Input type="password" name="confirmpassword" id="confirmpassword" required />
                    </FormGroup>

                    <Button type="button" className="float-right" color="primary" onClick={this.register} disabled={this.state.disableButton}>注册</Button>
                </Form>
            </div>
        );
    }
}