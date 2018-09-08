import React, { Component } from "react";
import { Form, FormGroup, Label, Input, Button, Modal, ModalHeader, ModalBody, ModalFooter, FormText } from "reactstrap";
import { FormPost } from "../../../utils/HttpRequest";

export class ProblemEditor extends Component {
    displayName = ProblemEditor.name

    constructor(props) {
        super(props);
        this.generateProblemEditor = this.generateProblemEditor.bind(this);
        this.saveProblem = this.saveProblem.bind(this);
        this.toggleModal = this.toggleModal.bind(this);

        this.state = {
            disabled: false,
            showModal: false
        };
        this.script = 0;
    }

    toggleModal() {
        var current = this.state.showModal;
        if (current) {
            while (this.script !== 0) {
                var ele = document.getElementById('script_block_' + (this.script--));
                if (ele !== null) ele.remove();
            }
        }
        else {
            var script = document.getElementById('script').value;
            var rawScript = '';
            script.split('\n').forEach(v => {
                if (v.startsWith('[jslib]:')) {
                    var libele = document.createElement('script');
                    libele.id = 'script_block_' + (++this.script);
                    libele.src = v.substring(8);
                    document.body.appendChild(libele);
                }
                else {
                    rawScript += v;
                }
            });
            var ele = document.createElement('script');
            ele.id = 'script_block_' + (++this.script);
            ele.innerHTML = rawScript;
            document.body.appendChild(ele);
        }
        this.setState({
            showModal: !current
        });
    }

    generateProblemEditor(problem) {
        return (<Form id='pbmForm'>
            <Input type="hidden" name="pid" value={problem.id} />
            <FormGroup>
                <Label for="title">标题</Label>
                <Input type="text" defaultValue={problem.title} name="title" id="title" required />
            </FormGroup>
            <FormGroup>
                <Label for="content">内容 (HTML)</Label><a href="javascript:void(0)" onClick={this.toggleModal}>预览</a>
                <textarea className="form-control" defaultValue={problem.content} name="content" id="content" required />
            </FormGroup>
            <FormGroup>
                <Label for="script">脚本 (JavaScript)</Label>
                <FormText>如需引用 JavaScript 库请在单独的一行中填写 [jslib]:url，如[jslib]:https://sample.com/sample.js</FormText>
                <textarea className="form-control" defaultValue={problem.script} name="script" id="script" required />
            </FormGroup>
            <FormGroup>
                <Label for="level">等级</Label>
                <Input type="number" defaultValue={problem.level} name="level" id="level" min="1" max="10" />
            </FormGroup>
        </Form>);
    }

    saveProblem() {
        var form = document.getElementById('pbmForm');
        if (form.reportValidity()) {
            this.setState({
                disabled: true
            });
            FormPost('/Admin/NewProblemAsync', form)
                .then(res => res.json())
                .then(data => {
                    this.setState({
                        disabled: false
                    });
                    if (data.succeeded) {
                        alert('保存成功');
                        window.location = '/Account/Admin/ProblemManager';
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
        let problem = this.props.problem === null ? this.generateProblemEditor({ id: 0, time: '', title: '', content: '', level: 0 }) : this.generateProblemEditor(this.props.problem);

        return (
            <div>
                {problem}
                <br />
                <Button className="float-right" color="primary" onClick={this.saveProblem} disabled={this.state.disabled}>保存</Button>

                <Modal isOpen={this.state.showModal} toggle={this.toggleModal}>
                    <ModalHeader toggle={this.toggleModal}>{this.state.showModal ? document.getElementById('title').value : null}</ModalHeader>
                    <ModalBody dangerouslySetInnerHTML={{ __html: this.state.showModal ? document.getElementById('content').value : '' }} />
                    <ModalFooter><span className="float-right">等级：{this.state.showModal ? document.getElementById('level').value : null}</span></ModalFooter>
                </Modal>
            </div>
        );
    }
}