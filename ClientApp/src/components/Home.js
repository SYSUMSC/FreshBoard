import React, { Component } from 'react';
import { Jumbotron, Button, Container } from 'reactstrap';

export class Home extends Component {
  displayName = Home.name

  render() {
    return (
      <div>
        <Jumbotron>
          <Container>
            <h2 className="display-3">欢迎上车！</h2>
            <p className="lead">这里是中山大学微软学生俱乐部 —— 中山大学最 cool 的社团</p>
            <hr />
            <p>2018 的小朋友们，策划部、媒传部、技术部等你来加入</p>
            <Button color="primary">了解更多</Button>
          </Container>
        </Jumbotron>
      </div>
    );
  }
}
