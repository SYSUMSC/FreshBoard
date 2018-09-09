import React, { Component } from "react";
import { ModalHeader, ModalBody, ModalFooter, Modal, Container, Badge, Button, ListGroup, ListGroupItem, FormGroup, Input, Label } from "reactstrap";
import { ProblemEditor } from "./PoblemEditor";
import { Get, Post } from "../../../utils/HttpRequest";

export class ProblemManager extends Component {
    displayName = ProblemManager.name

    constructor(props) {
        super(props);
        this.state = {
            problems: [],
            loading: true,
            currentPage: 1,
            readIndex: 0,
            showModal: false
        };
        this._scrollHandler = this._scrollHandler.bind(this);
        this.newProblem = this.newProblem.bind(this);
        this.togglepbm = this.togglepbm.bind(this);
        this.removeProblem = this.removeProblem.bind(this);

        this.getProblems();
    }

    componentDidMount() {
        window.addEventListener('scroll', this._scrollHandler);
    }

    componentWillUnmount() {
        window.removeEventListener('scroll', this._scrollHandler);
    }

    _scrollHandler() {
        if (window.scrollY + window.innerHeight > document.body.offsetHeight) {
            if (this.state.loading) return;
            const t = this.state.currentPage;
            this.setState({
                currentPage: this.state.problems.length === 0 ? t : t + 1,
                loading: true
            });
            this.getProblems();
        }
    }

    getProblems() {
        Get('/Admin/GetProblemsAsync', {}, { start: (this.state.currentPage - 1) * 20, count: 20 })
            .then(response => response.json())
            .then(data => {
                if (data.succeeded)
                    this.setState({ problems: this.state.problems.concat(data.problems), loading: false });
                else alert(data.message);
            })
            .catch(() => alert('加载失败'));
    }

    togglepbm(index = 0) {
        if (this.state.showModal) {
            this.setState({ readIndex: 0, showModal: false });
            return;
        }
        this.setState({ readIndex: index, showModal: true });
    }
    
    removeProblem(pid, index) {
        Post('/Admin/RemoveProblemAsync', {}, { pid: pid })
            .then(res => res.json())
            .then(data => {
                if (data.succeeded) {
                    var ele = document.getElementById('pbm_' + index.toString());
                    if (ele !== null) ele.remove();
                }
                else alert(data.message);
            })
            .catch(() => alert('发生未知错误'));
    }

    getProblemLayout(problems) {
        return (
            <div>
                {
                    problems.length === 0 ? <p>暂无题目</p> :
                        <ListGroup>
                            {
                                problems.map((x, i) => (
                                    <ListGroupItem className="justify-content-between" id={"pbm_" + i.toString()}>
                                        <a href="javascript:void(0)" onClick={() => this.togglepbm(i, x.id)}>{x.title}</a>&nbsp;
                                <Badge pill>Level {x.level}</Badge>
                                        <div className="float-right">
                                            <Button color="danger" onClick={() => this.removeProblem(x.id, i)}>删除</Button>
                                        </div>
                                    </ListGroupItem>))
                            }
                        </ListGroup>
                }
            </div>
        );
    }

    newProblem() {
        this.setState({
            readIndex: this.state.problems.length,
            showModal: true
        });
    }

    render() {
        let canRead = this.state.problems.length > this.state.readIndex;

        return (
            <Container>
                <br />
                <h2>题目管理 <Button color="primary" onClick={this.newProblem}>新建</Button></h2>
                {this.getProblemLayout(this.state.problems)}
                {this.state.loading ? <div><br /><p>加载中...</p></div> : null}

                <Modal isOpen={this.state.showModal} toggle={this.togglepbm}>
                    <ModalHeader toggle={this.togglepbm}>编辑题目</ModalHeader>
                    <ModalBody>
                        <ProblemEditor problem={canRead ? this.state.problems[this.state.readIndex] : null} />
                    </ModalBody>
                    <ModalFooter><span className="float-right">SYSU MSC Problem System</span></ModalFooter>
                </Modal>

            </Container>
        );
    }
}