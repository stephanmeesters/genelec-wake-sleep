Name:           genelec-cli
Version:        1.0
Release:        1%{?dist}
Summary:        Genelec CLI app

License:        GPL
URL:            https://stephanmeesters.nl
Source0:        %{name}-%{version}.tar.gz

BuildRequires:  cargo
BuildRequires:  hidapi-devel
BuildRequires:  systemd-devel
Requires:       systemd

%define debug_package %{nil}

%description
Manage your Genelec speakers over CLI.

%prep
%autosetup

%build
rustup toolchain install stable-x86_64-unknown-linux-gnu && rustup run stable cargo build --release

%install
# Install binary
install -D -m 0755 target/release/genelec-cli %{buildroot}%{_bindir}/genelec-cli

# Install systemd services
install -D -m 0644 systemd/genelec-sleep.service %{buildroot}%{_unitdir}/genelec-sleep.service
install -D -m 0644 systemd/genelec-wake.service %{buildroot}%{_unitdir}/genelec-wake.service

# Install udev rule
install -D -m 0644 udev/70-geneleccli-hid.rules %{buildroot}%{_udevrulesdir}/70-geneleccli-hid.rules

%post
# Reload systemd and enable services
%systemd_post genelec-sleep.service
%systemd_post genelec-wake.service

%postun
# Clean up systemd units
%systemd_postun_with_restart genelec-sleep.service
%systemd_postun_with_restart genelec-wake.service

%preun
# Disable and stop systemd services
%systemd_preun genelec-sleep.service
%systemd_preun genelec-wake.service

%files
%license LICENSE
%doc README.md
%{_bindir}/genelec-cli
%{_unitdir}/genelec-sleep.service
%{_unitdir}/genelec-wake.service
%{_udevrulesdir}/70-geneleccli-hid.rules
