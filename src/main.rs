use anyhow::{Context, Result};
use clap::{Parser, Subcommand};
use hidapi::HidApi;

use genelec_cli::constants::*;
use genelec_cli::gnet_message::GNetMessage;
use genelec_cli::transport::Transport;

#[derive(Parser)]
#[command(
    name = "Genelec CLI tool",
    about = "Control Genelec GLM adapter via USB HID"
)]
struct Cli {
    #[command(subcommand)]
    action: Action,
}

#[derive(Subcommand)]
enum Action {
    Wake,
    Sleep,
}

fn main() -> Result<()> {
    let cli = Cli::parse();
    let api = HidApi::new().context("Failed to initialize HID API")?;
    let device = find_glm_device(&api).context("USB device could not be found.")?;

    match cli.action {
        Action::Wake => wake_up(device)?,
        Action::Sleep => shutdown(device)?,
    }
    Ok(())
}

fn find_glm_device(api: &HidApi) -> Option<hidapi::HidDevice> {
    api.device_list()
        .find(|info| info.vendor_id() == GENELEC_GLM_VID && info.product_id() == GENELEC_GLM_PID)
        .and_then(|info| api.open_path(info.path()).ok())
}

fn wake_up(device: hidapi::HidDevice) -> Result<()> {
    let transport = Transport::new(device);
    for data in &[[3, 0x7F], [3, 1]] {
        let msg = GNetMessage::with_data(GNET_BROADCAST_ADDR, CID_WAKEUP, data);
        for _ in 0..=1 {
            transport
                .send(&msg)
                .context("Failed to send wake message")?;
        }
    }
    Ok(())
}

fn shutdown(device: hidapi::HidDevice) -> Result<()> {
    let transport = Transport::new(device);
    for data in &[[3, 2], [3, 0]] {
        let msg = GNetMessage::with_data(GNET_BROADCAST_ADDR, CID_WAKEUP, data);
        for _ in 0..=1 {
            transport
                .send(&msg)
                .context("Failed to send shutdown message")?;
        }
    }
    Ok(())
}
