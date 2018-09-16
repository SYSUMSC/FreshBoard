import React, { Component } from 'react';
import { Get, Post } from '../utils/HttpRequest';
import { Container, Card, CardBody, CardTitle, CardSubtitle, Button, CardText, CardLink, Badge, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

export class Notification extends Component {
    displayName = Notification.name;

    constructor(props) {
        super(props);
        this.state = {
            notifications: [],
            loading: true,
            currentPage: 1,
            readIndex: 0,
            showModal: false
        };
        this._scrollHandler = this._scrollHandler.bind(this);
        this.togglemsg = this.togglemsg.bind(this);
        this.dismissmsg = this.dismissmsg.bind(this);
        this.getNotifications();
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
                currentPage: this.state.notifications.length === 0 ? t : t + 1,
                loading: true
            });
            this.getNotifications();
        }
    }

    getNotificationLayout(notifications) {
        return (
            <div>
                {
                    notifications.length === 0 ? <p>暂无通知</p> :
                        notifications.map((x, i) => (<Card>
                            <CardBody>
                                <CardTitle>{x.hasRead ? null : <Badge pill color="danger" id={'new_' + i.toString()}>未读</Badge>} {x.title}</CardTitle>
                                <CardSubtitle>{x.time}</CardSubtitle>
                            </CardBody>
                            <CardBody>
                                <CardText>{x.preview}</CardText>
                                <CardLink>
                                    <Button color="primary" onClick={() => this.togglemsg(i, x.id)}>查看详情</Button>
                                </CardLink>
                                {
                                    x.hasRead ? null :
                                        <CardLink id={'dismiss_' + i.toString()}>
                                            <Button color="secondary" onClick={() => this.dismissmsg(i, x.id)}>标记已读</Button>
                                        </CardLink>
                                }
                            </CardBody>
                        </Card>))
                }
            </div>
        );
    }

    togglemsg(index = 0, element = 0) {
        if (this.state.showModal) {
            this.setState({ readIndex: 0, showModal: false });
            return;
        }
        this.setState({ readIndex: index, showModal: true });
        if (element !== 0)
            this.dismissmsg(index, element);
    }

    dismissmsg(index = 0, element = 0) {
        if (element !== 0) {
            Post('/Notification/DismissNotificationAsync', {}, { nid: element });
            const newBadge = document.getElementById('new_' + index.toString());
            const dismissBadge = document.getElementById('dismiss_' + index.toString());
            if (newBadge !== null) newBadge.remove();
            if (dismissBadge !== null) dismissBadge.remove();
        }
    }

    getNotifications() {
        Get('/Notification/GetNotificationsAsync', {}, { start: (this.state.currentPage - 1) * 10, count: 10 })
            .then(response => response.json())
            .then(data => this.setState({ notifications: this.state.notifications.concat(data), loading: false }))
            .catch(() => alert('加载失败'));
    }

    render() {
        let canRead = this.state.notifications.length > this.state.readIndex;
        let loginPrompt = this.props.user === null ? null :
            this.props.user.isSignedIn ? null : <p className="text-info">请登录查看更多通知</p>;
        return (
            <Container>
                <br />
                <h2>通知中心</h2>
                {loginPrompt}
                {this.getNotificationLayout(this.state.notifications)}
                {this.state.loading ? <div><br /><p>加载中...</p></div> : null}

                <Modal isOpen={this.state.showModal} toggle={this.togglemsg}>
                    <ModalHeader toggle={this.togglemsg}>{canRead ? this.state.notifications[this.state.readIndex].title : null}</ModalHeader>
                    <ModalBody dangerouslySetInnerHTML={{ __html: canRead ? this.state.notifications[this.state.readIndex].content : '' }} />
                    <ModalFooter><span className="float-right">{canRead ? this.state.notifications[this.state.readIndex].time : null}</span></ModalFooter>
                </Modal>
            </Container>
        );
    }
}