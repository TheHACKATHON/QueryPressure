import {ConnectionStringProps} from "./ConnectionStringProps";
import React, {useState} from "react";
import {EyeIcon, EyeSlashIcon} from "../assets/Icons";

function ConnectionString(props: ConnectionStringProps) {
  const [connectionStringShown, setConnectionStringState] = useState<boolean>(false);

  function getInputType(): "text" | "password" {
    return connectionStringShown ? "text" : "password";
  }

  function getValidationClass() {
    return props.validationMessage.isGood ? 'valid-feedback text-truncate w-100' : 'invalid-feedback text-truncate w-100';
  }

  return (
    <div>
      <div className="input-group">
        <label htmlFor="connectionString" className="form-label">Connection string</label>
      </div>
      <div className="input-group mb-3 is-invalid is-valid">
        <input type={getInputType()} className="form-control" id="connectionString"
               onChange={(event) => props.changed(event.target.value)}/>
        <button className="btn btn-outline-secondary" type="button"
                onClick={() => props.test()}>
          Test
        </button>
        <button className="btn btn-outline-secondary" type="button"
                onClick={() => setConnectionStringState(!connectionStringShown)}>
          {!connectionStringShown && <EyeIcon/>}
          {connectionStringShown && <EyeSlashIcon/>}
        </button>
      </div>
      {props.validationMessage &&
          <div className={getValidationClass()} title={props.validationMessage.message}>
            {props.validationMessage.message}
          </div>
      }
    </div>
  );
}

export default ConnectionString;
