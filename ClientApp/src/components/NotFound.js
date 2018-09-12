import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class NotFound extends Component {
    displayName = NotFound.name

    render() {
        return (
            <Container>
                <br />
                <h2>404</h2>
                <p>这里什么都没有哦</p>
            </Container>
        );
    }
}
