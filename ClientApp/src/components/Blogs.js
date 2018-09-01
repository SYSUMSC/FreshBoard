import React, { Component } from 'react';
import { Get } from '../utils/HttpRequest';
import Button from 'reactstrap/lib/Button';
import Marked from 'marked';
import Highlight from 'highlight.js';
import 'highlight.js/styles/github.css';

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
            fileTime: ''
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
                content: '',
                fileTree: response.fileList
            });
        });

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
                        fileTime: response.time
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
        let parentFolder = this.state.path === '' ? null : <p>Folder: <a href="javascript:void(0)" onClick={() => this.loadBlogs("..", 0)}>..</a></p>;

        let content = this.state.loading ? <em>Loading...</em> :
            this.state.displayContent ?
                <div>
                    <p>{this.state.fileName} - {this.state.fileTime}</p>
                    <div dangerouslySetInnerHTML={{ __html: Marked(this.state.content) }} />
                    <hr />
                    <Button color="primary" onClick={() => this.setState({ displayContent: false })}>Return</Button>
                </div> :
                <div> {parentFolder} {this.state.fileTree.map(i =>
                    <p><span>{i.type === 0 ? "Folder" : "File"}: </span><a href="javascript:void(0)" onClick={() => this.loadBlogs(i.fileName, i.type)}>{i.fileName}</a></p>
                )} </div>;
        
        return (
            <div>
                <p>Current path: /{this.state.path}</p>
                {content}
            </div>
        );
    }
}