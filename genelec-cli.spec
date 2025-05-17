Name:           genelec-cli
Version:        1.0
Release:        1%{?dist}
Summary:        Genelec CLI app

License:        GPL
URL:            https://stephanmeesters.nl
Source0:        %{name}-%{version}.tar.gz

BuildRequires:  cargo
Requires:       systemd

%description
Small Rust application to manage Genelec speaker power states via systemd and udev.

%prep
%autosetup

%build
cargo build --release

%install
# Install binary
install -D -m 0755 target/release/GenelecCli %{buildroot}%{_bindir}/GenelecCli

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
%{_bindir}/GenelecCli
%{_unitdir}/genelec-sleep.service
%{_unitdir}/genelec-wake.service
%{_udevrulesdir}/70-geneleccli-hid.rules

%changelog
* Sat May 17 2025 Stephan Meesters <stephan.meesters@gmail.com> - 1.0-1
- Initial package

