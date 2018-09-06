import React, { Component } from "react";
import { Container, FormGroup, Label, Input, Form, Button } from "reactstrap";
import { SearchUsers } from "./SearchUsers";
import { FormPost } from "../../../utils/HttpRequest";

export class PrivilegeManager extends Component {
    displayName = PrivilegeManager.name

    constructor(props) {
        super(props);
        this.state = {
            disabled: false
        };
        this.savePrivilege = this.savePrivilege.bind(this);
    }

    savePrivilege() {
        var form = document.getElementById('priForm');
        if (form.reportValidity()) {
            this.setState({ disabled: true });
            FormPost('/Admin/ModifyPrivilegeAsync', form)
                .then(res => res.json())
                .then(data => {
                    this.setState({
                        disabled: false
                    });
                    if (data.succeeded) {
                        alert('保存成功');
                    }
                    else {
                        alert(data.message);
                        this.setState({
                            disabled: false
                        });
                    }
                })
                .catch(() => {
                    alert('发生未知错误');
                    this.setState({
                        disabled: false
                    });
                });
        }
    }

    render() {
        return (
            <Container>
                <h2>权限管理</h2>
                <Form id='priForm'>
                    <FormGroup>
                        <Label>用户 Id</Label>
                        <Input name='userId' type='text' required />
                    </FormGroup>
                    <FormGroup>
                        <Label>新权限</Label>&nbsp;
                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="privilege" id="pri0" value="0" defaultChecked />普通用户
                            </label>
                        </div>

                        <div className="form-check-inline">
                            <label className="form-check-label">
                                <Input type="radio" name="privilege" id="pri1" value="1" />管理员
                            </label>
                        </div>
                    </FormGroup>
                    <br />
                    <h4>成员信息搜索</h4>
                    <SearchUsers />
                    <br />
                    <p><Button className="float-right" color="primary" onClick={this.savePrivilege} disabled={this.state.disabled}>保存</Button></p>
                </Form>
                <br />
            </Container>
        );
    }
}