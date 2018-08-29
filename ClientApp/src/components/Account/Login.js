import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";
import { DOMAIN } from "../../config";

export class Login extends Component {
    displayName = Login.name

    render() {
        return (
            <div>
                <Form method="post" action={`${DOMAIN}/Account/LoginAsync`}>
                    <FormGroup>
                        <Label for="email">电子邮箱</Label>
                        <Input type="email" name="email" id="email" />
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