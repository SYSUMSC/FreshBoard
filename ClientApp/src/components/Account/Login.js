import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";

export class Login extends Component {
    displayName = Login.name

    render() {
        return (
            <div>
                <Form method="post" action='/Account/LoginAsync'>
                    <FormGroup>
                        <Label for="email">电子邮箱</Label>
                        <Input type="email" name="email" id="email" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="persistent">记住登录状态</Label>&nbsp;
                        <div class="form-check-inline">
                            <label class="form-check-label">
                                <Input type="radio" name="persistent" id="persistent" />
                            </label>
                        </div>
                    </FormGroup>

                    <Button className="float-right" color="primary">登录</Button>
                </Form>
            </div>
        );
    }
}