import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { HeartrateGraph } from './components/HeartrateGraph';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={HeartrateGraph} />
      </Layout>
    );
  }
}
