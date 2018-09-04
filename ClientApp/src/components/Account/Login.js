import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input } from "reactstrap";
import { FormPost } from "../../utils/HttpRequest";

export class Login extends Component {
    displayName = Login.name

    constructor(props) {
        super(props);
        this.login = this.login.bind(this);

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

                    <Button type="button" className="float-right" color="primary" onClick={this.login} disabled={this.state.disableButton}>登录</Button>
                </Form>
            </div>
        );
    }
}