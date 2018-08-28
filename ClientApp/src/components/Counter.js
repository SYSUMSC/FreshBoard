import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { Button } from 'reactstrap';

export class Counter extends Component {
    displayName = Counter.name

    constructor(props) {
        super(props);
        this.state = { currentCount: 0 };
        this.incrementCounter = this.incrementCounter.bind(this);

    }

    incrementCounter() {
        this.setState({
            currentCount: this.state.currentCount + 1
        });
    }

    render() {
        return (
            <div>
                <Container>
                    <h1>Counter</h1>

                    <p>This is a simple example of a React component.</p>

                    <p>Current count: <strong>{this.state.currentCount}</strong></p>

                    <Button bsStyle="primary" onClick={this.incrementCounter}>Increment</Button>
                </Container>
            </div>
        );
    }
}
