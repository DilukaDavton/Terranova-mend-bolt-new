import App from "./components/App";
import { AppContainer } from "react-hot-loader";
import { initializeIcons } from "@fluentui/font-icons-mdl2";
import { ThemeProvider } from "@fluentui/react";
import * as React from "react";
import * as ReactDOM from "react-dom";
/* global document, Office, module, require */

initializeIcons();

let isOfficeInitialized = false;

const title = "Terranova Add-in";

const render = (Component) => {
  ReactDOM.render(
    <AppContainer>
      <ThemeProvider>
        <Component title={title} isOfficeInitialized={isOfficeInitialized} />
      </ThemeProvider>
    </AppContainer>,
    document.getElementById("container")
  );
};

/* Render application after Office initializes */
Office.initialize = () => {
  isOfficeInitialized = true;
  render(App);
};

(async () => {
  setTimeout(function () {
    if (isOfficeInitialized == false) {
      let count = localStorage.getItem("refreshCount");
      if (count === null) {
        localStorage.setItem("refreshCount", "1");
        window.location.href = `${window.location.href}`;
      } else if (+count < 3) {
        localStorage.setItem("refreshCount", `${+count + 1}`);
        window.location.href = `${window.location.href}`;
      }
    } else {
      localStorage.removeItem("refreshCount");
    }
  }, 10000);
})();

if ((module as any).hot) {
  (module as any).hot.accept("./components/App", () => {
    const NextApp = require("./components/App").default;
    render(NextApp);
  });
}
