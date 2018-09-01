import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";
import { Post } from "../../utils/HttpRequest";


export class Register extends Component {
    displayName = Register.name

    render() {
        return (
            <div>
                <Form method="post" action='/Account/RegisterAsync'>
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
                        <Input type="text" name="phone" id="phone" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="wechat">WeChat</Label>
                        <Input type="text" name="wechat" id="wechat" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="qq">QQ</Label>
                        <Input type="text" name="qq" id="qq" />
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
                    </FormGroup>

                    <FormGroup>
                        <Label for="schnum">学号</Label>
                        <Input type="text" name="schnum" id="schnum" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="institute">学院</Label>
                        <Input type="text" name="institute" id="institute" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="major">专业</Label>
                        <Input type="text" name="major" id="major" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="password">密码</Label>
                        <Input type="password" name="password" id="password" />
                    </FormGroup>

                    <FormGroup>
                        <Label for="confirmpassword">确认密码</Label>
                        <Input type="password" name="confirmpassword" id="confirmpassword" />
                    </FormGroup>

                    <Button className="float-right" color="primary" onClick={this.register}>注册</Button>
                </Form>
            </div>
        );
    }
}