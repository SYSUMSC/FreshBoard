import React, { Component } from "react";
import { Button, Input, Label, ListGroup, ListGroupItem } from "reactstrap";
import { Post } from "../../../utils/HttpRequest";

export class SearchUsers extends Component {
  displayName = SearchUsers.name;

  constructor(props) {
    super(props);

    this.searchUsers = this.searchUsers.bind(this);

    this.state = {
      loading: false,
      succeeded: false,
      users: [],
    };

    this.timer = null;
  }

  generateUserInfo() {
    if (this.state.users === null || this.state.users.length === 0)
      return <p>没有数据</p>;
    return (
      <div>
        <ListGroup>
          {this.state.users.map(x => (
            <ListGroupItem className="justify-content-between" key={x.id}>
              <p>
                {x.name} {x.sexual === 1 ? "♂" : "♀"}{" "}
                {x.department === 1
                  ? "行政策划部"
                  : x.department === 2
                    ? "媒体宣传部"
                    : x.department === 3
                      ? "综合技术部"
                      : "暂无部门"}{" "}
                {x.applyStatus === 1
                  ? "等待一面"
                  : x.applyStatus === 2
                    ? "等待二面"
                    : x.applyStatus === 3
                      ? "录取失败"
                      : x.applyStatus === 4
                        ? "录取成功"
                        : "暂无"}{" "}
                解谜进度: {x.crackProgress}
                <Button
                  className="float-right"
                  color="primary"
                  onClick={() => this.props.add(x.id)}
                >
                  添加
                </Button>
              </p>
              <p>
                Id: {x.id}{" "}
                <a href={"/Account/Identity?userId=" + x.id} target="_blank">
                  查看详情
                </a>
              </p>
              <p>
                {x.email} {x.emailConfirmed ? "√" : "×"} {x.phoneNumber}{" "}
                {x.phoneNumberConfirmed ? "√" : "×"}
              </p>
            </ListGroupItem>
          ))}
        </ListGroup>
      </div>
    );
  }

  searchUsers() {
    if (this.timer !== null) clearTimeout(this.timer);
    this.timer = setTimeout(
      function() {
        this.setState({
          loading: true,
        });
        Post(
          "/Admin/SearchUsersAsync",
          {},
          { patterns: document.getElementById("patterns").value },
        )
          .then(res => res.json())
          .then(data => {
            if (data.succeeded) {
              this.setState({
                loading: false,
                users: data.users,
                succeeded: data.succeeded,
              });
            } else {
              this.setState({
                loading: false,
              });
              alert(data.message);
            }
          })
          .catch(() => alert("发生未知错误"));
      }.bind(this),
      250,
    );
  }

  render() {
    let userInfo = this.state.loading ? (
      <p>加载中...</p>
    ) : this.state.succeeded ? (
      this.generateUserInfo()
    ) : (
      <p>没有数据</p>
    );
    return (
      <div>
        <Label for="patterns">
          可通过用户 Id/姓名/邮箱/手机搜索成员，输入 $All 查看全部成员，输入
          $Record 查看已有面试记录的成员
        </Label>
        <Input id="patterns" type="text" onInput={this.searchUsers} />
        <br />
        {userInfo}
      </div>
    );
  }
}
