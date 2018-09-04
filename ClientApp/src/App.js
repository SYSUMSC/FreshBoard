import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Blogs } from './components/Blogs';
import { Notification } from './components/Notification';
import { Get } from './utils/HttpRequest';
import { Portal } from './components/Account/Portal';
import { ConfirmEmail } from './components/Account/ConfirmEmail';

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
            })
            .catch(() => alert('用户信息获取失败'));

    }

    setTitle(title) {
        document.title = title + " - SYSU MSC";
    }

    render() {
        return (
            <Layout user={this.state.user}>
                <Route exact path='/' render={() => { this.setTitle('主页'); return <Home user={this.state.user} />; }} />
                <Route exact path='/Notification' render={() => { this.setTitle('通知'); return <Notification user={this.state.user} />; }} />
                <Route exact path='/Blogs' render={() => { this.setTitle('干货'); return <Blogs user={this.state.user} />; }} />
                <Route exact path='/Account/Portal' render={() => { this.setTitle('我的账户'); return <Portal user={this.state.user} />; }} />
                <Route path='/Account/ConfirmEmail' render={() => { this.setTitle('验证邮箱'); return <ConfirmEmail user={this.state.user} />; }} />
            </Layout>
        );
    }
}
