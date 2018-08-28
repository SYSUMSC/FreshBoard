import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";

export class Register extends Component {
    displayName = Register.name

    render() {
        return (
            <div>
                <Form method="post" action="/Account/Register">
                    <FormGroup>
                        <Label for="username">用户名</Label>
                        <Input type="text" name="username" id="username" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="email">电子邮箱</Label>
                        <Input type="email" name="email" id="email" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="phone">电话</Label>
                        <Input type="number" name="phone" id="phone" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="qq">QQ</Label>
                        <Input type="number" name="qq" id="qq" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" />
                    </FormGroup>

                    <Button className="float-right" color="primary">注册</Button>
                </Form>
            </div>
        );
    }
}