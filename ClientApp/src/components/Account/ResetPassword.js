import React, { Component } from "react";
import { Container, Form, FormGroup, Label, Input, Button } from "reactstrap";
import { FormPost } from "../../utils/HttpRequest";

export class ResetPassword extends Component {
    displayName = ResetPassword.name

    constructor(props) {
        super(props);
        this.reset = this.reset.bind(this);
        this.state = {
            disabled: false
        };
    }

    reset() {
        var form = document.getElementById('resetForm');
        if (form !== null && form.reportValidity()) {
            this.setState({ disabled: true });
            FormPost('/Account/ResetPasswordAsync', form)
                .then(res => res.json())
                .then(data => {
                    if (data.succeeded) {
                        alert('重置成功');
                        window.location = '/';
                    }
                    else {
                        alert(data.message);
                        this.setState({ disabled: false });
                    }
                })
                .catch(() => {
                    alert('重置失败');
                    this.setState({ disabled: false });
                });
        }
    }

    render() {
        var query = window.location.search.substring(1).split('&');
        var userId = '';
        var code = '';
        for (var i = 0; i < query.length; i++) {
            var str = query[i];
            var index = str.indexOf('=');
            if (index >= 0) {
                var identifier = str.substring(0, index);
                var value = str.substring(index + 1);
                switch (identifier) {
                    case "userId":
                        userId = value;
                        break;
                    case "code":
                        code = value;
                        break;
                }
            }
        }

        return (
            <Container>
                <br />
                <h2>重置密码</h2>
                <Form id="resetForm" method="post">
                    <Input type="hidden" name="userId" id="userId" value={userId} />
                    <Input type="hidden" name="token" id="token" value={code} />
                    <FormGroup>
                        <Label for="password">新密码</Label>
                        <Input id="password" type="password" name="password" required />
                    </FormGroup>
                    <FormGroup>
                        <Label for="confirmpassword">确认密码</Label>
                        <Input id="confirmpassword" type="password" name="confirmpassword" required />
                    </FormGroup>
                    <Button color="primary" className="float-right" disabled={this.state.disabled} onClick={this.reset}>确定</Button>
                    <br />
                </Form>
            </Container>
        );
    }
}