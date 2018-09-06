import React, { Component } from "react";
import { Button, Input, FormGroup } from "reactstrap";
import { Post } from '../../utils/HttpRequest';

export class ConfirmPhone extends Component {
    displayName = ConfirmPhone.name;

    constructor(props) {
        super(props);
        this.sendConfirmSMS = this.sendConfirmSMS.bind(this);
        this.confirmPhone = this.confirmPhone.bind(this);

        this.state = {
            succeeded: false,
            sent: false,
            confirmed: false,
            disabled: false
        };
    }

    sendConfirmSMS() {
        this.setState({ disabled: true });
        Post('/Account/SendSMSAsync', {}, {})
            .then(res => res.json())
            .then(data => {
                if (data.succeeded)
                    this.setState({
                        sent: true
                    });
                else {
                    alert(data.message);
                    this.setState({
                        sent: false,
                        disabled: false
                    });
                }
            }).catch(() => {
                this.setState({ disabled: false });
                alert('发生错误，请稍后再试');
            });
    }

    confirmPhone() {
        const token = document.getElementById('confirmToken').value;

        Post('/Account/ConfirmPhoneAsync', {}, { token: token })
            .then(res => res.json())
            .then(data => {
                if (data.succeeded)
                    this.setState({
                        succeeded: true,
                        confirmed: true
                    });
                else {
                    alert(data.message);
                    this.setState({
                        succeeded: false,
                        confirmed: true
                    });
                }
            }).catch(() => {
                this.setState({ disabled: false, sent: false });
                alert('发生错误，请稍后再试');
            });
    }

    render() {
        let buttonText = this.state.sent ? "已发送" : "发送验证短信";

        let confirmPanel = this.state.confirmed ?
            (this.state.succeeded ? <p className="text-success">验证成功</p> : <p className="text-danger">验证失败</p>)
            : (<div>
                <p>你的电话号码尚未验证，请点击发送验证短信进行验证</p>

                <table style={{ width: '100%' }}>
                    <tr>
                        <td>
                            <Input type="number" id="confirmToken" />
                        </td>
                        <td className="float-right">
                            <Button color="primary" disabled={this.state.disabled} onClick={this.sendConfirmSMS}>{buttonText}</Button>
                        </td>
                    </tr>
                </table>
                <br />
                <Button color="primary" className="float-right" onClick={this.confirmPhone}>确定</Button>
            </div>);

        return (
            <div>
                <br />
                <h2>验证电话号码</h2>
                {confirmPanel}
            </div>
        );
    }
}