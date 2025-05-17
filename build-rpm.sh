#!/bin/bash
set -e

NAME=genelec-cli
VERSION=1.0
TARBALL=${NAME}-${VERSION}.tar.gz
RPMBUILD_DIR=~/rpmbuild
LOCAL_BUILD_DIR=../${NAME}-${VERSION}

echo "🛠️ Building Rust project..."
cargo build --release

echo "📦 Creating source tarball..."
mkdir ${LOCAL_BUILD_DIR}
cp -r . ${LOCAL_BUILD_DIR}/
tar czf ${TARBALL} ${LOCAL_BUILD_DIR}
rm -rf ${LOCAL_BUILD_DIR}

echo "📁 Moving tarball to SOURCES..."
rpmdev-setuptree
mv ${TARBALL} ${RPMBUILD_DIR}/SOURCES/
cp ${NAME}.spec ${RPMBUILD_DIR}/SPECS/

echo "📦 Building RPM..."
rpmbuild -ba ${RPMBUILD_DIR}/SPECS/${NAME}.spec

echo "✅ Done. RPMs are in:"
find ${RPMBUILD_DIR}/{RPMS,SRPMS} -name "*.rpm"
