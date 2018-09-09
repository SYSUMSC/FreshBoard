import React, { Component } from "react";
import { Button, Form, FormGroup, Label, Input, FormText } from "reactstrap";
import { FormPost } from "../../utils/HttpRequest";

export class Apply extends Component {
    displayName = Apply.name

    constructor(props) {
        super(props);
        this.apply = this.apply.bind(this);
        this.fillinDefaultInfo = this.fillinDefaultInfo.bind(this);

        this.state = {
            disableButton: false
        };
    }

    apply() {
        var form = document.getElementById('applyForm');
        if (form.reportValidity()) {
            this.setState({ disableButton: true });
            FormPost('/Account/ApplyAsync', form)
                .then(response => response.json())
                .then(data => {
                    if (data.succeeded) {
                        window.location = '/Account/Portal';
                    }
                    else {
                        alert(data.message);
                        this.setState({ disableButton: false });
                    }
                })
                .catch(() => { alert('申请失败'); this.setState({ disableButton: false }); });
        }
    }

    fillinDefaultInfo(userInfo) {
        return (
            <Form id="applyForm">
                <FormGroup>
                    <Label for="department">部门</Label>&nbsp;
                        <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="department" id="department0" value="0" defaultChecked={userInfo.department === 0} />
                            退部
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="department" id="department1" value="1" defaultChecked={userInfo.department === 1} />
                            行政策划部
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="department" id="department2" value="2" defaultChecked={userInfo.department === 2} />
                            媒体宣传部
                            </label>
                    </div>
                    <div className="form-check-inline">
                        <label className="form-check-label">
                            <Input type="radio" name="department" id="department3" value="3" defaultChecked={userInfo.department === 3} />
                            综合技术部
                            </label>
                    </div>
                    <FormText color="danger">注意：修改申请将会重置录取状态，请谨慎操作</FormText>
                </FormGroup>


                <Button type="button" className="float-right" color="primary" onClick={this.apply} disabled={this.state.disableButton}>确定</Button>
            </Form>);
    }

    render() {
        let applyPanel = this.props.user === null ? <p>加载中...</p> :
            this.props.user.isSignedIn ?
                this.fillinDefaultInfo(this.props.user.userInfo) : <p>没有数据</p>;
        return (
            <div>
                {applyPanel}
            </div>
        );
    }
}