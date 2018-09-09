import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input } from "reactstrap";
import { Post } from "../../utils/HttpRequest";
import { SerializeForm } from "../../utils/FormHelper";

export class ModifyOther extends Component {
    displayName = ModifyOther.name;

    constructor(props) {
        super(props);
        this.modify = this.modify.bind(this);
        this.fillinDefaultInfo = this.fillinDefaultInfo.bind(this);

        this.state = {
            disableButton: false
        };
    }

    fillinDefaultInfo(otherInfo) {
        if (otherInfo === null) return null;
        return (
            <Form id='modifyForm'>
                {
                    otherInfo.map(x => (<FormGroup>
                        <Label for={x.key}>{x.description}</Label>
                        <textarea className="form-control" name={x.key} id={x.key} defaultValue={x.value} />
                    </FormGroup>))
                }

                <Button type="button" className="float-right" color="primary" onClick={this.modify} disabled={this.state.disableButton}>修改</Button>
            </Form>);
    }

    modify() {
        var form = document.getElementById('modifyForm');
        if (form.reportValidity()) {
            this.setState({ disableButton: true });
            Post('/Account/ModifyOtherAsync', {}, { data: JSON.stringify(SerializeForm(form)) })
                .then(response => response.json())
                .then(data => {
                    if (data.succeeded) {
                        this.props.updateStatus();
                        this.props.closeModal();
                    }
                    else {
                        alert(data.message);
                        this.setState({ disableButton: false });
                    }
                })
                .catch(() => { alert('修改失败'); this.setState({ disableButton: false }); });
        }
    }

    render() {
        let modifyPanel = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                this.fillinDefaultInfo(this.props.user.otherInfo) : <p>没有数据</p>;

        return (
            <div>
                {modifyPanel}
            </div>
        );
    }
}