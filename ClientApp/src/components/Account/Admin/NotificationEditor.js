import React, { Component } from "react";
import { Form, FormGroup, Label, Input, Button, FormText, Modal, ModalHeader, ModalBody, ModalFooter } from "reactstrap";
import { SearchUsers } from "./SearchUsers";
import { FormPost } from "../../../utils/HttpRequest";

export class NotificationEditor extends Component {
    displayName = NotificationEditor.name

    constructor(props) {
        super(props);
        this.generateNotificationEditor = this.generateNotificationEditor.bind(this);
        this.setHint = this.setHint.bind(this);
        this.saveNotification = this.saveNotification.bind(this);
        this.previewNotification = this.previewNotification.bind(this);
        this.toggleModal = this.toggleModal.bind(this);

        this.state = {
            targetsHint: '',
            disabled: false,
            showModal: false
        };
    }

    setHint(mode) {
        switch (mode) {
            case 0:
                this.setState({ targetsHint: '此模式下消息不会推送给任何人' });
                break;
            case 1:
                this.setState({ targetsHint: '此模式下消息会推送给全体成员' });
                break;
            case 2:
                this.setState({ targetsHint: '此模式下消息会推送给指定部门的成员，请填写部门编号 1 -- 行政策划部，2 -- 媒体宣传部, 3 -- 综合技术部' });
                break;
            case 3:
                this.setState({ targetsHint: '此模式下消息会推送给指定成员，请填写成员 Id' });
                break;
            case 4:
                this.setState({ targetsHint: '此模式下消息会推送给指定权限的成员，请填写权限编号 0 -- 普通成员，1 -- 管理员' });
                break;
        }
    }

    previewNotification() {
        this.toggleModal();
    }

    toggleModal() {
        this.setState({
            showModal: !this.state.showModal
        });
    }

    generateNotificationEditor(notification) {
        if (this.state.targetsHint === '') {
            this.setHint(notification.mode);
            return;
        }
        return (<Form id='notiForm'>
            <Input type="hidden" name="nid" value={notification.id} />
            <FormGroup>
                <Label for="title">标题</Label>
                <Input type="text" defaultValue={notification.title} name="title" id="title" required />
            </FormGroup>
            <FormGroup>
                <Label for="content">内容 (HTML)</Label><a href="javascript:void(0)" onClick={this.previewNotification}>预览</a>
                <textarea className="form-control" defaultValue={notification.content} name="content" id="content" required />
            </FormGroup>
            <FormGroup>
                <Label for="time">推送时间</Label>
                <Input type="datetime-local" defaultValue={notification.time} name="time" id="time" />
            </FormGroup>
            <FormGroup>
                <Label for="mode">推送模式</Label>&nbsp;
                <div className="form-check-inline">
                    <label className="form-check-label">
                        <Input type="radio" name="mode" id="mode0" value="0" defaultChecked={notification.mode === 0} onChange={() => this.setHint(0)} />无
                    </label>
                </div>
                <div className="form-check-inline">
                    <label className="form-check-label">
                        <Input type="radio" name="mode" id="mode1" value="1" defaultChecked={notification.mode === 1} onChange={() => this.setHint(1)} />全体成员
                    </label>
                </div>

                <div className="form-check-inline">
                    <label className="form-check-label">
                        <Input type="radio" name="mode" id="mode2" value="2" defaultChecked={notification.mode === 2} onChange={() => this.setHint(2)} />指定部门
                    </label>
                </div>

                <div className="form-check-inline">
                    <label className="form-check-label">
                        <Input type="radio" name="mode" id="mode3" value="3" defaultChecked={notification.mode === 3} onChange={() => this.setHint(3)} />指定用户
                    </label>
                </div>

                <div className="form-check-inline">
                    <label className="form-check-label">
                        <Input type="radio" name="mode" id="mode4" value="4" defaultChecked={notification.mode === 4} onChange={() => this.setHint(4)} />指定权限
                    </label>
                </div>
            </FormGroup>
            <FormGroup>
                <Label for="targets">推送目标</Label>
                <FormText>{this.state.targetsHint}，多个项目请用 | 分隔</FormText>
                <textarea className="form-control" defaultValue={notification.targets} name="targets" />
            </FormGroup>
        </Form>);
    }

    saveNotification() {
        var form = document.getElementById('notiForm');
        if (form.reportValidity()) {
            this.setState({
                disabled: true
            });
            FormPost('/Admin/NewNotificationAsync', form)
                .then(res => res.json())
                .then(data => {
                    this.setState({
                        disabled: false
                    });
                    if (data.succeeded) {
                        this.props.updateList();
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
        let notification = this.props.notification === null ? this.generateNotificationEditor({ id: 0, time: '', title: '', content: '', mode: 0, targets: '', hasPushed: false }) : this.generateNotificationEditor(this.props.notification);

        return (
            <div>
                {notification}
                <br />
                <h4>成员信息搜索</h4>
                <SearchUsers />
                <br />
                <Button className="float-right" color="primary" onClick={this.saveNotification} disabled={this.state.disabled}>保存</Button>

                <Modal isOpen={this.state.showModal} toggle={this.toggleModal}>
                    <ModalHeader toggle={this.toggleModal}>{this.state.showModal ? document.getElementById('title').value : null}</ModalHeader>
                    <ModalBody dangerouslySetInnerHTML={{ __html: this.state.showModal ? document.getElementById('content').value : '' }} />
                    <ModalFooter><span className="float-right">{this.state.showModal ? document.getElementById('time').value : null}</span></ModalFooter>
                </Modal>
            </div>
        );
    }
}