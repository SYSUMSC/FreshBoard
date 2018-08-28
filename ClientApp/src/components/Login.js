import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";

export class Login extends Component {
    displayName = Login.name

    render() {
        return (
            <div>
                <Form method="post" action="/Account/Login">
                    <FormGroup>
                        <Label for="username">用户名/邮箱</Label>
                        <Input type="text" name="username" id="username" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" />
                    </FormGroup>

                    <Button className="float-right" color="primary">登录</Button>
                </Form>
            </div>
        );
    }
}