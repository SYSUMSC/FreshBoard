import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input } from "reactstrap";
import { FormPost, Post } from "../../utils/HttpRequest";

export class Login extends Component {
    displayName = Login.name

    constructor(props) {
        super(props);
        this.login = this.login.bind(this);
        this.forgetPassword = this.forgetPassword.bind(this);

        this.state = {
            disableButton: false
        };
    }

    login() {
        var form = document.getElementById('loginForm');
        if (form.reportValidity()) {
            this.setState({ disableButton: true });
            FormPost('/Account/LoginAsync', form)
                .then(response => response.json())
                .then(data => {
                    if (data.succeeded) window.location = '/';
                    else {
                        alert(data.message);
                        this.setState({ disableButton: false });
                    }
                })
                .catch(() => { alert('登录失败'); this.setState({ disableButton: false }); });
        }
    }

    forgetPassword() {
        var email = document.getElementById('email');
        if (email === null || email.value === null || email.value === '' || !(email.value.indexOf('@') > 0)) {
            alert('请填写正确的电子邮箱地址');
            return;
        }
        Post('/Account/SendResetPasswordEmailAsync', {}, { email: email.value })
            .then(res => res.json())
            .then(data => {
                if (data.succeeded) {
                    alert('已发送一封关于重置密码的邮件到您的邮箱，请根据邮件说明重置密码');
                }
                else {
                    alert(data.message);
                }
            })
            .catch(() => alert('发生未知错误'));
    }

    render() {
        return (
            <div>
                <Form id="loginForm">
                    <FormGroup>
                        <Label for="email">电子邮箱</Label>
                        <Input type="email" name="email" id="email" required />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" required />
                    </FormGroup>

                    <FormGroup>
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="checkbox" name="persistent" id="persistent" />
                                记住登录状态
                            </label>
                        </div>
                    </FormGroup>
                    <p>
                        <a href="javascript:void(0)" onClick={this.forgetPassword}>重置密码</a>
                        <Button type="button" className="float-right" color="primary" onClick={this.login} disabled={this.state.disableButton}>登录</Button>
                    </p>
                </Form>
            </div>
        );
    }
}