function login() {
  if (!validateLoginForm()) {
    return;
  }

  const email = $("#Email").val();
  const password = $("#Password").val();
  const rememberMe = $("#RememberMe").prop("checked");
  const returnUrl = "";
  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Email: email,
      Password: password,
      RememberMe: rememberMe,
      ReturnUrl: returnUrl,
    },
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        setTimeout(function() {
            window.location.href = response.redirectUrl;
        }, 2000);
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function register() {
  if (!validateRegisterForm()) {
    return;
  }

  const email = $("#Email").val();
  const password = $("#Password").val();
  const confirmPassword = $("#ConfirmPassword").val();


  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Email: email,
      Password: password,
      ConfirmPassword: confirmPassword
    },
    success: function (response) {
      if (response.success) {
        toastr.info(response.message);
        setTimeout(function() {
          window.location.href = response.redirectUrl;
        }, 2000);
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function resetPassword(){

  if(!validateResetPasswordForm()){
    return;
  }

  const password = $("#Password").val();
  const confirmPassword = $("#ConfirmPassword").val();

  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Password: password,
      ConfirmPassword: confirmPassword
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
  
}

function VerifyOTP(){
  if(!validateOTPForm()){
    return;
  }

  const otp = $("#OTP").val();

  $.ajax({
    url: Url,
    type: "POST",
    data: {
      OTP: otp
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    },
    error: function (xhr, status, error) {
      toastr.error("An error occurred: " + error);
    },
  });
}

function SendEmail(){
  if(!validateEmailForm()){
    return;
  }

  const email = $("#Email").val();

  $.ajax({
    url: Url,
    type: "POST",
    data: {
      Email: email
    },
    success: function (response) {
      if (response.success) {
        window.location.href = response.redirectUrl;
      } else {
        toastr.error(response.message);
      }
    }
  })
}

function validateLoginForm() {
  const email = document.getElementById("Email").value.trim();
  const password = document.getElementById("Password").value;
  const valid = true;

  if (!email) {
    document.getElementById("emailError").textContent =
      "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent =
      "Please enter a valid email address.";
    valid = false;
  }

  if (!password) {
    document.getElementById("passwordError").textContent =
      "Please enter your password.";
    valid = false;
  }

  return valid;
}

function validateRegisterForm() {
  const email = document.getElementById("Email").value.trim();
  const password = document.getElementById("Password").value;
  const confirmPassword = document.getElementById("ConfirmPassword").value;
  const passwordValidation = validatePassword(password);
  const valid = true;

  if (!email) {
    document.getElementById("emailError").textContent = "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent = "Please enter a valid email address.";
    valid = false;
  }

  if (!password) {
    document.getElementById("passwordError").textContent = "Please enter your password.";
    valid = false;
  } else if(!passwordValidation.success) {
    document.getElementById("passwordError").textContent = passwordValidation.message;
    valid = false;
  }

  if (!confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Please confirm your password.";
    valid = false;
  } else if (password !== confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Confirmed password does not match.";
    valid = false;
  }

  return valid;

}

function validateResetPasswordForm() {
  const password = document.getElementById("Password").value;
  const confirmPassword = document.getElementById("ConfirmPassword").value;
  const passwordValidation = validatePassword(password);  
  const valid = true;

  if (!password) {
    document.getElementById("passwordError").textContent = "Please enter your password.";
    valid = false;
  } else if(!passwordValidation.success) {
    document.getElementById("passwordError").textContent = passwordValidation.message;
    valid = false;
  }

  if (!confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Please confirm your password.";
    valid = false;
  } else if (password !== confirmPassword) {
    document.getElementById("confirmPasswordError").textContent = "Confirmed password does not match.";
    valid = false;
  }

  return valid;
}

function validateEmailForm() {
  const email = document.getElementById("Email").value.trim();
  const valid = true;

  if (!email) {
    document.getElementById("emailError").textContent = "Please enter your email.";
    valid = false;
  } else if (!validateEmail(email)) {
    document.getElementById("emailError").textContent = "Please enter a valid email address.";
    valid = false;
  }

  return valid;
}

function validatePassword(password) {

  const minLength = 8;
  const maxLength = 20;
  const hasUpperCase = /[A-Z]/;
  const hasLowerCase = /[a-z]/;
  const hasNumber = /\d/;
  const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/;
  const messages = [];

  if (password.length < minLength || password.length > maxLength) {
    messages.push(`be ${minLength}-${maxLength} characters long`);
  }
  if (!hasUpperCase.test(password)) {
    messages.push("contain at least one uppercase letter");
  }
  if (!hasLowerCase.test(password)) {
    messages.push("contain at least one lowercase letter");
  }
  if (!hasNumber.test(password)) {
    messages.push("contain at least one number");
  }
  if (!hasSpecialChar.test(password)) {
    messages.push("contain at least one special character");
  }

  if (messages.length > 0) {
    return { success: false, message: `Password must ${messages.join(", ")}.` };
  }
  return { success: true, message: null };
}

function validateEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

function validateOTPForm() {
  const otp = document.getElementById("OTP").value;
  const valid = true;

  if (!otp) { 
    document.getElementById("OTPError").textContent = "Please enter your OTP.";
    valid = false;
  }

  return valid;
}

document.getElementById("Email")?.addEventListener("input", function () {
  document.getElementById("emailError").textContent = "";
})

document.getElementById("Password")?.addEventListener("input", function () {
  document.getElementById("passwordError").textContent = "";
})

document.getElementById("ConfirmPassword")?.addEventListener("input", function () {
    document.getElementById("confirmPasswordError").textContent = "";
})

document.getElementById("OTP")?.addEventListener("input", function () {
  document.getElementById("OTPError").textContent = "";
})

