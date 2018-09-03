import React, { Component } from 'react';
import { Get } from '../utils/HttpRequest';
import Marked from 'marked';
import Highlight from 'highlight.js';
import 'highlight.js/styles/github.css';
import { Container, Breadcrumb, Button, BreadcrumbItem, Alert, Fade, ListGroup, ListGroupItem, Badge } from 'reactstrap';
import 'github-markdown-css';

export class Blogs extends Component {
    displayName = Blogs.name;

    constructor(props) {
        super(props);

        this.state = {
            displayContent: false,
            loading: true,
            path: '',
            content: '',
            fileTree: [],
            fileName: '',
            fileTime: '',
            author: '',
            subject: ''
        };

        Marked.setOptions({
            highlight: (code) => Highlight.highlightAuto(code).value,
            sanitize: false
        });

        //load root directory
        Get('/Blog/GetBlogTree', {}, { path: this.state.path }).then(data => data.json()).then(response => {
            this.setState({
                displayContent: false,
                loading: false,
                path: response.currentPath,
                fileTree: response.fileList
            });
        });

        //Add mathjax library from external cdn
        let script = document.createElement('script');
        script.src = 'https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.5/MathJax.js?config=TeX-MML-AM_CHTML';
        document.body.appendChild(script);
    }

    loadBlogs(path, type) {
        //process path string
        var newPath = '';
        if (path === '..') {
            if (this.state.path !== '')
                newPath = this.state.path.substring(0, this.state.path.lastIndexOf('/'));
        }
        else newPath = this.state.path + '/' + path;
        if (newPath === '/') newPath = '';
        while (newPath.startsWith('/')) newPath = newPath.substring(1);

        //init
        this.setState({
            displayContent: false,
            loading: true
        });

        if (type === 0) { //directory
            Get('/Blog/GetBlogTree', {}, { path: newPath })
                .then(data => data.json())
                .then(response => {
                    this.setState({
                        displayContent: false,
                        loading: false,
                        content: '',
                        fileName: '',
                        fileTime: '',
                        author: '',
                        subject: '',
                        path: response.currentPath,
                        fileTree: response.fileList
                    });
                });
        }
        else { //file
            Get('/Blog/GetBlogContentAsync', {}, { path: newPath })
                .then(data => data.json())
                .then(response => {
                    this.setState({
                        displayContent: true,
                        loading: false,
                        content: response.content,
                        fileName: response.name,
                        fileTime: response.time,
                        author: response.author,
                        subject: response.subject
                    });
                    if (window.MathJax) {
                        window.MathJax.Hub.Config({
                            extensions: ["jsMath2jax.js"],
                            tex2jax: {
                                inlineMath: [['$', '$'], ["\\(", "\\)"]],
                                displayMath: [['$$', '$$'], ["\\[", "\\]"]]
                            }
                        });
                        window.MathJax.Hub.Configured();
                        window.MathJax.Hub.Queue(["Typeset", window.MathJax.Hub, "output"]);
                    }
                });
        }
    }

    render() {
        let parentFolder = this.state.path === '' ? null : <ListGroupItem className="justify-content-between"><a href="javascript:void(0)" onClick={() => this.loadBlogs("..", 0)}>返回上级目录</a><span>&nbsp;</span><Badge pill>目录</Badge></ListGroupItem>;

        let content = this.state.loading ? <p>加载中...</p> :
            this.state.displayContent ?
                <div>
                    <Alert color="info">
                        <h4 className="alert-heading">{this.state.fileName.substring(0, this.state.fileName.lastIndexOf('.'))}<Button className="float-right" color="primary" onClick={() => this.setState({ displayContent: false })}>返回</Button></h4>
                        <p>最后更新：{this.state.subject}</p>
                        <hr />
                        <p className="mb-0">作者：{this.state.author}</p>
                        <p className="mb-0">时间：{this.state.fileTime}</p>
                    </Alert>
                    <Fade in>
                        <div className="markdown-body" dangerouslySetInnerHTML={{ __html: Marked(this.state.content) }} />
                    </Fade>
                    <hr />
                </div> :
                <div>
                    <ListGroup>
                        {this.state.fileTree.map(i => {
                            return (
                                <ListGroupItem className="justify-content-between">
                                    <a href="javascript:void(0)" onClick={() => this.loadBlogs(i.fileName, i.type)}>{i.type === 0 ? i.fileName : i.fileName.substring(0, i.fileName.lastIndexOf('.'))}</a>
                                    <span>&nbsp;</span>
                                    <Badge pill>{i.type === 0 ? "目录" : "文档"}
                                    </Badge>
                                </ListGroupItem>);
                        })}
                        {parentFolder}
                    </ListGroup>
                </div>;

        return (
            <Container>
                <br />
                <Breadcrumb>
                    {('主页/' + (this.state.path === '' ? '' : this.state.path + '/')).split('/').map((x, i, arr) => i === arr.length - 1 ? null : i === arr.length - 2 ? <BreadcrumbItem active>{x}</BreadcrumbItem> : <BreadcrumbItem>{x}</BreadcrumbItem>)}
                </Breadcrumb>
                {content}
            </Container>
        );
    }
}