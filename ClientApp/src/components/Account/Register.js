import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";
import {DOMAIN} from "../../config";


export class Register extends Component {
    displayName = Register.name

    render() {
        return (
            <div>
                <Form method="post" action={`${DOMAIN}/Account/RegisterAsync`}>
                    <FormGroup>
                        <Label for="name">姓名</Label>
                        <Input type="text" name="name" id="name" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="email">电子邮箱</Label>
                        <Input type="email" name="email" id="email" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="grade">年级</Label>
                        <Input type="number" name="grade" id="grade" min="2015" max="2018" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="phone">电话</Label>
                        <Input type="string" name="phone" id="phone" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="wechat">WeChat</Label>
                        <Input type="string" name="wechat" id="wechat" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="qq">QQ</Label>
                        <Input type="string" name="qq" id="qq" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="sexual">性别</Label>
                        <Input type="number" name="sexual" id="sexual" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="cpclevel">政治面貌</Label>
                        <Input type="number" name="cpclevel" id="cpclevel" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="schnum">学号</Label>
                        <Input type="string" name="schnum" id="schnum" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="institute">学院</Label>
                        <Input type="string" name="institute" id="institute" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="majority">专业</Label>
                        <Input type="string" name="majority" id="majority" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="confirmpassword">确认密码</Label>
                        <Input type="password" name="confirmpassword" id="confirmpassword" />
                    </FormGroup>

                    <Button className="float-right" color="primary">注册</Button>
                </Form>
            </div>
        );
    }
}