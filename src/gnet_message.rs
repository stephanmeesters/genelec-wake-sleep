use crate::constants::GNET_TERM;

pub struct GNetMessage {
    pub message: Vec<u8>,
}

impl GNetMessage {
    pub fn with_data(address: u8, command: u8, data: &[u8]) -> Self {
        let mut buf = Vec::with_capacity(8 + data.len());
        buf.push(address);
        buf.push(command);
        buf.extend_from_slice(data);
        let crc = calculate_crc16_gsm(&buf);
        buf.extend_from_slice(&crc);
        buf.push(GNET_TERM);
        GNetMessage { message: buf }
    }
}

fn calculate_crc16_gsm(data: &[u8]) -> [u8; 2] {
    let mut crc: u16 = 0x0000;
    for &b in data {
        crc ^= (b as u16) << 8;
        for _ in 0..8 {
            if (crc & 0x8000) != 0 {
                crc = (crc << 1) ^ 0x1021;
            } else {
                crc <<= 1;
            }
        }
    }
    crc ^= 0xFFFF;
    crc.to_be_bytes()
}
