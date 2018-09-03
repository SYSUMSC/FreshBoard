import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Blogs } from './components/Blogs';
import { Notification } from './components/Notification';
import { Get } from './utils/HttpRequest'
import { Portal } from './components/Account/Portal';

export default class App extends Component {
    displayName = App.name

    constructor(props) {
        super(props);

        this.state = {
            user: null
        };

        Get('/Account/GetUserInfoAsync')
            .then(data => data.json())
            .then(data => {
                this.setState({ user: data });
            });

    }

    setTitle(title) {
        document.title = title + " - SYSU MSC";
    }

    render() {
        return (
            <Layout user={this.state.user}>
                <Route exact path='/' render={() => { this.setTitle('主页'); return <Home user={this.state.user} />; }} />
                <Route path='/Notification' render={() => { this.setTitle('通知'); return <Notification user={this.state.user} />; }} />
                <Route path='/Blogs' render={() => { this.setTitle('干货'); return <Blogs user={this.state.user} />; }} />
                <Route path='/Portal' render={() => { this.setTitle('我的账户'); return <Portal user={this.state.user} />; }} />
            </Layout>
        );
    }
}
