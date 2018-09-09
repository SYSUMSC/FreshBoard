import React, { Component } from 'react';
import { Container, FormGroup, Label, Button, Alert } from 'reactstrap';
import { Get, Post } from '../utils/HttpRequest';

export class Crack extends Component {
    displayName = Crack.name

    constructor(props) {
        super(props);

        this.getProblem = this.getProblem.bind(this);
        this.renderProblem = this.renderProblem.bind(this);
        this.submit = this.submit.bind(this);
        this.onDismiss = this.onDismiss.bind(this);

        this.state = {
            problem: null,
            loading: true,
            disabled: false,
            visible: true
        };

        this.script = 0;

        this.getProblem();
    }

    onDismiss() {
        this.setState({ visible: false });
    }

    getProblem() {
        this.setState({ loading: true });
        Get('/Problem/GetProblemAsync', {}, {})
            .then(res => res.json())
            .then(data => {
                if (data.succeeded) {
                    this.setState({ problem: data.problem, loading: false });
                }
                else {
                    alert(data.message);
                    this.setState({ problem: null, loading: false });
                }
            })
            .catch(() => {
                alert("发生未知错误");
                this.setState({ problem: null, loading: false });
            });
    }

    submit(pid) {
        var answer = document.getElementById('answer');
        if (answer !== null) {
            this.setState({ disabled: true });
            Post('/Problem/SubmitAnswerAsync', {}, { pid: pid, answer: answer.value })
                .then(res => res.json())
                .then(data => {
                    if (data.succeeded) {
                        this.updateStatus();
                        this.getProblem();
                    }
                    else {
                        alert(data.message);
                    }
                    this.setState({ disabled: false });
                })
                .catch(() => {
                    alert("发生未知错误");
                    this.setState({ disabled: false });
                });
        }
    }

    renderProblem(problem) {
        if (problem === null) return <p>没有数据</p>;
        else {
            while (this.script !== 0) {
                var eleTmp = document.getElementById('script_block_' + (this.script--));
                if (eleTmp !== null) eleTmp.remove();
            }
            var script = problem.script;
            if (script === null) script = '';
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

            return (
                <div>
                    <h4>Level {problem.level}: {problem.title}</h4>
                    <hr />
                    <div dangerouslySetInnerHTML={{ __html: problem.content }}></div>
                    <hr />
                    <FormGroup>
                        <Label for="answer">答案</Label>
                        <textarea id="answer" className="form-control" />
                        <br />
                        <p><Button color="primary" className="float-right" onClick={() => this.submit(problem.id)} disabled={this.state.disabled}>确定</Button></p>
                    </FormGroup>
                    <br />
                </div>
            );
        }
    }

    render() {
        let problem = this.state.loading ? <p>加载中...</p> : this.renderProblem(this.state.problem);

        return (
            <Container>
                <br />
                <h2>解谜</h2>
                <Alert color="info" isOpen={this.state.visible} toggle={this.onDismiss}>
                    完成所有题目之后可以免试进入二面呦~
                </Alert>
                {problem}
            </Container>
        );
    }
}